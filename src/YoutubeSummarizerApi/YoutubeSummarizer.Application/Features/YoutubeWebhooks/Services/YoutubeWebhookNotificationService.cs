using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YoutubeSummarizer.Application.Common.Interfaces;
using YoutubeSummarizer.Application.Features.AI.PromptTemplates;
using YoutubeSummarizer.Application.Features.Notifications.Dtos;
using YoutubeSummarizer.Application.Features.Notifications.Interfaces;
using YoutubeSummarizer.Application.Features.UserAccount.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Dtos;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Models;
using YoutubeSummarizer.Domain.Enums;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Services
{
    public class YoutubeWebhookNotificationService : IYoutubeWebhookNotificationService
    {
        private readonly IYoutubeChannelRepository _channelRepo;
        private readonly IUserYoutubeChannelSubscriptionRepository _subscriptionRepo;
        private readonly IYoutubeWebhookNotificationParser _notificationParser;
        private readonly IYoutubeTranscriptClient _transcriptClient;
        private readonly IBlacklistEntryRepository _blacklistRepo;
        private readonly IAiClient _aiClient;
        private readonly ITranscriptPromptTemplateProvider _promptTemplateProvider;
        private readonly INotificationService _notificationService;
        private readonly IYoutubeMetadataClient _metadataClient;
        private readonly ILogger<YoutubeWebhookNotificationService> _logger;

        public YoutubeWebhookNotificationService(
            IYoutubeChannelRepository channelRepo,
            IUserYoutubeChannelSubscriptionRepository subscriptionRepo,
            IYoutubeWebhookNotificationParser notificationParser,
            IYoutubeTranscriptClient transcriptClient,
            IBlacklistEntryRepository blacklistRepo,
            IAiClient aiClient,
            ITranscriptPromptTemplateProvider promptTemplateProvider,
            INotificationService notificationService,
            IYoutubeMetadataClient metadataClient,
            ILogger<YoutubeWebhookNotificationService> logger)
        {
            _channelRepo = channelRepo;
            _subscriptionRepo = subscriptionRepo;
            _notificationParser = notificationParser;
            _transcriptClient = transcriptClient;
            _blacklistRepo = blacklistRepo;
            _aiClient = aiClient;
            _promptTemplateProvider = promptTemplateProvider;
            _notificationService = notificationService;
            _metadataClient = metadataClient;
            _logger = logger;
        }

        public async Task ProcessNotificationAsync(string payload, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Webhook received. Raw payload:\n{Payload}", payload);

            var notification = _notificationParser.Parse(payload);
            var videoUrl = $"https://www.youtube.com/watch?v={notification.VideoId}";
            var videoTitle = await ResolveVideoTitleAsync(notification, videoUrl, cancellationToken);

            _logger.LogInformation(
                "Parsed webhook: \"{VideoTitle}\" by {ChannelName} | Video: {VideoId} | Channel: {ChannelId}",
                videoTitle, notification.ChannelName, notification.VideoId, notification.ChannelId);

            var channel = await _channelRepo.GetByYoutubeChannelIdAsync(notification.ChannelId, cancellationToken);
            if (channel is null)
            {
                _logger.LogWarning("Received webhook for unknown channel {YoutubeChannelId}.", notification.ChannelId);
                return;
            }

            var subscriptions = await _subscriptionRepo.GetByYoutubeChannelIdAsync(channel.Id, cancellationToken);
            if (subscriptions.Count == 0)
            {
                _logger.LogWarning("Channel {ChannelId} has no subscribers.", notification.ChannelId);
                return;
            }

            var transcriptText = await FetchTranscriptWithRetriesAsync(notification.VideoId, videoUrl, cancellationToken);

            if (string.IsNullOrWhiteSpace(transcriptText))
            {
                _logger.LogInformation("No transcript available for {VideoId}. Sending fallback notifications.", notification.VideoId);
                var allUserIds = subscriptions.Select(s => s.UserId).Distinct().ToList();
                await SendNotificationsAsync(videoTitle, notification.ChannelName, "A summary of this video is not available.", allUserIds, null, cancellationToken);
                return;
            }

            var filteredSubscriptions = await ApplyBlacklistFilterAsync(subscriptions, transcriptText, cancellationToken);

            if (filteredSubscriptions.Count == 0)
            {
                _logger.LogInformation("All subscribers filtered out by blacklist for video {VideoId}.", notification.VideoId);
                return;
            }

            var styleGroups = filteredSubscriptions
                .GroupBy(s => s.SummarizationStyle)
                .ToDictionary(g => g.Key, g => g.Select(s => s.UserId).Distinct().ToList());

            _logger.LogInformation(
                "Summarizing video {VideoId} in {StyleCount} style(s) for {SubscriberCount} subscriber(s).",
                notification.VideoId, styleGroups.Count, filteredSubscriptions.Select(s => s.UserId).Distinct().Count());

            var completed = await GenerateSummariesAsync(styleGroups, transcriptText, cancellationToken);

            foreach (var (style, userIds, summary) in completed)
            {
                await SendNotificationsAsync(videoTitle, notification.ChannelName, summary, userIds, style, cancellationToken);
            }
        }

        private async Task<string> ResolveVideoTitleAsync(
            YoutubeWebhookNotification notification,
            string videoUrl,
            CancellationToken cancellationToken)
        {
            try
            {
                var metadata = await _metadataClient.GetVideoMetadataAsync(videoUrl, cancellationToken);
                return metadata.Title;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve fresh video title for {VideoId}, using XML title.", notification.VideoId);
                return notification.VideoTitle;
            }
        }

        private async Task<string?> FetchTranscriptWithRetriesAsync(
            string videoId,
            string videoUrl,
            CancellationToken cancellationToken)
        {
            var transcriptRequest = new GetYoutubeTranscriptRequest { VideoUrl = videoUrl };
            const int maxRetries = 3;

            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var transcriptResponse = await _transcriptClient.GetTranscriptAsync(transcriptRequest, cancellationToken);

                    if (transcriptResponse.Transcript.Count > 0)
                        return string.Join(" ", transcriptResponse.Transcript.Select(s => s.Text));

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Transcript fetch attempt {Attempt}/{MaxRetries} failed for video {VideoId}.",
                        attempt, maxRetries, videoId);

                    if (attempt < maxRetries)
                        await Task.Delay(2000 * attempt, cancellationToken);
                }
            }

            return null;
        }

        private async Task<List<UserYoutubeChannelSubscription>> ApplyBlacklistFilterAsync(
            List<UserYoutubeChannelSubscription> subscriptions,
            string transcriptText,
            CancellationToken cancellationToken)
        {
            var subscriberUserIds = subscriptions.Select(s => s.UserId).Distinct().ToList();
            var allBlacklistEntries = await _blacklistRepo.GetByUserIdsAsync(subscriberUserIds, cancellationToken);
            var subscriberBlacklists = allBlacklistEntries
                .GroupBy(e => e.UserId)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Keyword).ToList());

            var transcriptLower = transcriptText.ToLowerInvariant();

            return subscriptions
                .Where(s =>
                {
                    if (!subscriberBlacklists.TryGetValue(s.UserId, out var keywords) || keywords.Count == 0)
                        return true;
                    return !keywords.Any(k => transcriptLower.Contains(k.ToLowerInvariant()));
                })
                .ToList();
        }

        private async Task<List<(TranscriptSummarizationStyle Style, List<Guid> UserIds, string Summary)>> GenerateSummariesAsync(
            Dictionary<TranscriptSummarizationStyle, List<Guid>> styleGroups,
            string transcriptText,
            CancellationToken cancellationToken)
        {
            var styleList = styleGroups.ToList();
            var completed = new List<(TranscriptSummarizationStyle Style, List<Guid> UserIds, string Summary)>();
            var fallbackIndex = -1;

            for (var i = 0; i < styleList.Count; i++)
            {
                var (style, userIds) = styleList[i];
                var promptTemplate = _promptTemplateProvider.GetTemplate(style);
                var fullPrompt = $"{promptTemplate}\n\nTranscript:\n{transcriptText}";

                try
                {
                    var summary = await _aiClient.CompletePrimaryAsync(fullPrompt, cancellationToken);
                    completed.Add((style, userIds, summary));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Primary AI failed for {Style}, falling back to Ollama for remaining styles.", style);
                    fallbackIndex = i;
                    break;
                }
            }

            if (fallbackIndex >= 0)
            {
                var remaining = styleList.Skip(fallbackIndex).ToList();
                var fallbackPrompts = remaining
                    .Select(kvp => $"{_promptTemplateProvider.GetTemplate(kvp.Key)}\n\nTranscript:\n{transcriptText}")
                    .ToList();

                var fallbackTasks = fallbackPrompts
                    .Select(p => _aiClient.CompleteFallbackAsync(p, cancellationToken))
                    .ToList();

                string[] fallbackResults;
                try
                {
                    fallbackResults = await Task.WhenAll(fallbackTasks);
                }
                catch
                {
                    fallbackResults = fallbackTasks
                        .Select(t => t.IsCompletedSuccessfully ? t.Result : "A summary of this video is not available.")
                        .ToArray();
                }

                for (var i = 0; i < remaining.Count; i++)
                {
                    var (style, userIds) = remaining[i];
                    completed.Add((style, userIds, fallbackResults[i]));
                }
            }

            return completed;
        }

        private async Task SendNotificationsAsync(
            string videoTitle,
            string channelName,
            string content,
            List<Guid> userIds,
            TranscriptSummarizationStyle? style,
            CancellationToken cancellationToken)
        {
            await _notificationService.CreateNotificationAsync(new CreateNotificationRequest
            {
                Type = NotificationType.NewVideo,
                Title = videoTitle,
                Content = content,
                SenderName = channelName
            }, userIds, cancellationToken);

            if (style.HasValue)
                _logger.LogInformation(
                    "Sent {Style} summary notification for \"{VideoTitle}\" to {UserCount} user(s).",
                    style.Value, videoTitle, userIds.Count);
        }
    }
}