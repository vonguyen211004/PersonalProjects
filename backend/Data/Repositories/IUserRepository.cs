using System.Threading.Tasks;
using backend.Models;

namespace backend.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserWithProjectsAsync(int userId);
        Task<User> GetUserWithTasksAsync(int userId);
    }
}