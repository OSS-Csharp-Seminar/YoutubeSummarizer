using YoutubeSummarizer.Application.Common.Interfaces;
using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.Blacklist.Dtos;
using YoutubeSummarizer.Application.Features.Blacklist.Interfaces;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Features.Blacklist.Services
{
    public class BlacklistService : IBlacklistService
    {
        private const int MaxKeywordsPerUser = 50;

        private readonly IBlacklistRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public BlacklistService(IBlacklistRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<ServiceResponse<List<BlacklistedKeywordDto>>> GetKeywordsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId().ToString();
                var keywords = await _repository.GetByUserIdAsync(userId, cancellationToken);
                var dtos = keywords.Select(k => new BlacklistedKeywordDto
                {
                    Id = k.Id,
                    Keyword = k.Keyword,
                    CreatedAtUtc = k.CreatedAtUtc
                }).ToList();

                return ServiceResponse<List<BlacklistedKeywordDto>>.Success(dtos, "Keywords retrieved successfully.");
            }
            catch
            {
                return ServiceResponse<List<BlacklistedKeywordDto>>.Failure("An error occurred.");
            }
        }

        public async Task<ServiceResponse<BlacklistedKeywordDto>> AddKeywordAsync(AddKeywordRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId().ToString();
                var normalized = request.Keyword.Trim().ToLowerInvariant();

                var count = await _repository.CountByUserIdAsync(userId, cancellationToken);
                if (count >= MaxKeywordsPerUser)
                    return ServiceResponse<BlacklistedKeywordDto>.Failure($"You cannot add more than {MaxKeywordsPerUser} blacklisted keywords.");

                var exists = await _repository.ExistsAsync(userId, normalized, cancellationToken);
                if (exists)
                    return ServiceResponse<BlacklistedKeywordDto>.Failure("This keyword is already in your blacklist.");

                var entity = new BlacklistedKeyword
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Keyword = normalized,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _repository.AddAsync(entity, cancellationToken);

                return ServiceResponse<BlacklistedKeywordDto>.Success(new BlacklistedKeywordDto
                {
                    Id = entity.Id,
                    Keyword = entity.Keyword,
                    CreatedAtUtc = entity.CreatedAtUtc
                }, "Keyword added to blacklist.");
            }
            catch
            {
                return ServiceResponse<BlacklistedKeywordDto>.Failure("An error occurred.");
            }
        }

        public async Task<ServiceResponse<bool>> RemoveKeywordAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId().ToString();
                var keyword = await _repository.GetByIdAsync(id, cancellationToken);

                if (keyword is null || keyword.UserId != userId)
                    return ServiceResponse<bool>.Failure("Keyword not found.");

                await _repository.DeleteAsync(keyword, cancellationToken);
                return ServiceResponse<bool>.Success(true, "Keyword removed from blacklist.");
            }
            catch
            {
                return ServiceResponse<bool>.Failure("An error occurred.");
            }
        }
    }
}