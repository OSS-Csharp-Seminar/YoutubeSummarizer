using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Domain.Models;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;

namespace YoutubeSummarizer.Infrastructure.Persistence.Repositorys
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthRepository(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            var normalizedEmail = _userManager.NormalizeEmail(email);
            var applicationUser = await _db.Users
                .Include(x => x.DomainUser)
                .SingleOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail);

            return applicationUser?.DomainUser;
        }

        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<User> CreateUserAsync(string firstName, string lastName, string email, string password)
        {
            var domainUser = new User
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName,
                DomainUserId = domainUser.Id,
                DomainUser = domainUser,
                EmailConfirmed = true
            };

            _db.DomainUsers.Add(domainUser);

            var result = await _userManager.CreateAsync(applicationUser, password);

            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await _db.SaveChangesAsync();

            return domainUser;
        }
    }
}
