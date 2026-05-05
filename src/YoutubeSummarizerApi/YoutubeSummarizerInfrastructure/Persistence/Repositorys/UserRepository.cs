using System;
using System.Collections.Generic;
using System.Text;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Domain.Models;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;

namespace YoutubeSummarizer.Infrastructure.Persistence.Repositorys
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var identityUser = await _db.Users.FindAsync(id);

            if (identityUser == null)
                return null;

            return new User
            {
                Id = identityUser.Id,
                FullName = identityUser.FullName
            };
        }

        public async Task CreateAsync(User user)
        {
            var identityUser = new ApplicationUser
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.FullName.Replace(" ", "").ToLower()
            };

            _db.Users.Add(identityUser);
            await _db.SaveChangesAsync();
        }
    }

}
