using YoutubeSummarizer.Application.DTOs;

namespace YoutubeSummarizer.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto);
    }
}
