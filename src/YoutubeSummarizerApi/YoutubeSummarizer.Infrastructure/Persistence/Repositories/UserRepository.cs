using Microsoft.EntityFrameworkCore;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Domain.Models;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;

namespace YoutubeSummarizer.Infrastructure.Persistence.Repositories
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
            var user = await _db.DomainUsers.FindAsync(id);
            return user;
        }

        public async Task CreateAsync(User user)
        {
            _db.DomainUsers.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Guid>> GetAllActiveUserIdsAsync(CancellationToken cancellationToken = default)
            => await _db.Users
                .Where(u => u.DomainUser != null && u.DomainUser.IsActive)
                .Select(u => u.DomainUser!.Id)
                .ToListAsync(cancellationToken);

        public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _db.DomainUsers
                .OrderBy(u => u.Email)
                .ToListAsync(cancellationToken);

        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            _db.DomainUsers.Update(user);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}


