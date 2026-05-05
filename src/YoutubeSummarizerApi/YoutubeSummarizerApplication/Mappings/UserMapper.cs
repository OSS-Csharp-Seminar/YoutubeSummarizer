using YoutubeSummarizer.Application.DTOs;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Mappings
{
    public class UserMapper
    {
        public LoginResponseDto MapToLoginResponseDto(User user, string accessToken)
        {
            return new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                AccessToken = accessToken
            };
        }

        public RegisterResponseDto MapToRegisterResponseDto(User user, string accessToken)
        {
            return new RegisterResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                AccessToken = accessToken
            };
        }
    }
}

