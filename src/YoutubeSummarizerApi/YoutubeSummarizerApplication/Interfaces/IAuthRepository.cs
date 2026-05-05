using System;
using System.Collections.Generic;
using System.Text;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(string email, string password);
    }
}
