using WebAPI.Models;

namespace WebAPI.Interfaces
{
    public interface IUser
    {
        Task CreateUserAsync(User user);
        Task<User> GetUserAsync(int id);
        Task<IEnumerable<User>> GetUsersAsync();
        void UpdateUser(User user);
        Task DeleteUserAsync(int id);
        Task<User> AuthenticationAsync(string username, string password);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<bool> IsExistsAsync(int id);
        Task SaveAsync();
    }
}
