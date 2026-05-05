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
            var user = await _db.DomainUsers.FindAsync(id);
            return user;
        }

        public async Task CreateAsync(User user)
        {
            _db.DomainUsers.Add(user);
            await _db.SaveChangesAsync();
        }
    }
}


