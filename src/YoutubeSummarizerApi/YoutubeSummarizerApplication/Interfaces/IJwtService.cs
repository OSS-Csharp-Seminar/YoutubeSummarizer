using System;
using System.Collections.Generic;
using System.Text;
using YoutubeSummarizer.Application.DTOs;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }

    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    }
}
