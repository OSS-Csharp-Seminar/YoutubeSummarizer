using YoutubeSummarizer.Application.DTOs;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Mappings
{
    public class UserMapper
    {
        public LoginResponseDto MapToLoginResponseDto(User user, string accessToken, string refreshToken)
        {
            return new LoginResponseDto
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public RegisterResponseDto MapToRegisterResponseDto(User user, string accessToken, string refreshToken)
        {
            return new RegisterResponseDto
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
