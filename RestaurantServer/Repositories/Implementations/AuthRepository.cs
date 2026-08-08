using System.Data.Entity;
using System.Threading.Tasks;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;

namespace RestaurantServer.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task<User> GetUserByIdAsync(long userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Id == userId);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
