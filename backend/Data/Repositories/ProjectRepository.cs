using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;

namespace backend.Data.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        public async Task<Project> GetProjectWithDetailsAsync(int projectId)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Members)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.AssignedTo)
                .FirstOrDefaultAsync(p => p.Id == projectId);
        }
        
        public async Task<IEnumerable<Project>> GetUserProjectsAsync(int userId)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == userId || p.Members.Any(m => m.Id == userId))
                .ToListAsync();
        }
        
        public async Task<bool> AddUserToProjectAsync(int projectId, int userId)
        {
            var project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);
                
            var user = await _context.Users.FindAsync(userId);
            
            if (project == null || user == null)
                return false;
                
            if (project.Members.Any(m => m.Id == userId))
                return true; // User already in project
                
            project.Members.Add(user);
            return await SaveChangesAsync();
        }
        
        public async Task<bool> RemoveUserFromProjectAsync(int projectId, int userId)
        {
            var project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);
                
            if (project == null)
                return false;
                
            var user = project.Members.FirstOrDefault(m => m.Id == userId);
            
            if (user == null)
                return true; // User not in project
                
            project.Members.Remove(user);
            return await SaveChangesAsync();
        }
    }
}