namespace YoutubeSummarizer.Application.Features.Admin.Dtos
{
    public class AdminUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<AdminUserSubscriptionDto> Subscriptions { get; set; } = new();
    }

    public class AdminUserSubscriptionDto
    {
        public Guid SubscriptionId { get; set; }
        public string ChannelName { get; set; } = string.Empty;
        public string SummarizationStyle { get; set; } = string.Empty;
    }
}
