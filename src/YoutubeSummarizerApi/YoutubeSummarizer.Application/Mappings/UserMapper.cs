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
                User = MapToUserInfoResponseDto(user),
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public RegisterResponseDto MapToRegisterResponseDto(User user, string accessToken, string refreshToken)
        {
            return new RegisterResponseDto
            {
                User = MapToUserInfoResponseDto(user),
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public UserInfoResponseDto MapToUserInfoResponseDto(User user)
        {
            return new UserInfoResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}
