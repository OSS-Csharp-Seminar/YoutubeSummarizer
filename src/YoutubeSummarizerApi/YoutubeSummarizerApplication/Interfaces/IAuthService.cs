using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.DTOs;

namespace YoutubeSummarizer.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto, string ipAddress, CancellationToken cancellationToken = default);
        Task<ServiceResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto dto, string ipAddress, CancellationToken cancellationToken = default);
        Task<ServiceResponse<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto, string ipAddress, CancellationToken cancellationToken = default);
        Task LogoutAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default);
    }
}
