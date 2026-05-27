using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Application.Features.Notifications.Dtos
{
    public class GetNotificationsQuery
    {
        public NotificationType? Type { get; set; }
        public string? SenderSearch { get; set; }
        public bool SortDescending { get; set; } = true;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
