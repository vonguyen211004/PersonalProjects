using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using backend.Models;

namespace backend.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }
        
        public async Task<User> GetUserWithProjectsAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.OwnedProjects)
                .Include(u => u.Projects)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        
        public async Task<User> GetUserWithTasksAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.AssignedTasks)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}