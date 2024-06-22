using Microsoft.EntityFrameworkCore;
using WebAPI.Extensions;
using WebAPI.Interfaces;
using WebAPI.Models;

namespace WebAPI.Repositories
{
    public class UserRepository : IUser
    {
        private readonly WebAPIDbContext _context;
        public UserRepository(WebAPIDbContext context)
        {
            _context = context;
        }
        public async Task<User> AuthenticationAsync(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.Password == password.ToMD5Hash());
            return user;
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var dataToDelete = await _context.Users.FindAsync(id);
            if (dataToDelete == null)
                throw new ArgumentNullException(nameof(dataToDelete));
            
            _context.Users.Remove(dataToDelete); 
        }

        public async Task<User> GetUserAsync(int id)
        {
            var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> IsExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync(); 
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }
    }
}
