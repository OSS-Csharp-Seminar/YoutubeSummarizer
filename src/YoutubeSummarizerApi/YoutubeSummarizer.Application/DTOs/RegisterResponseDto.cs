namespace YoutubeSummarizer.Application.DTOs
{
    public class RegisterResponseDto
    {
        public UserInfoResponseDto User { get; set; } = new();
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
