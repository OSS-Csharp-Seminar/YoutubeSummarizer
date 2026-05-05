using System;
using System.Collections.Generic;
using System.Text;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task CreateAsync(User user);
    }
}
