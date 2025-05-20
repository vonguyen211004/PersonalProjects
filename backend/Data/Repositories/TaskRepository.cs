using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data.Repositories
{
    public class TaskRepository : Repository<TaskItem>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        public async Task<IEnumerable<TaskItem>> GetTasksByProjectIdAsync(int projectId)
        {
            return await _context.Tasks
                .Include(t => t.AssignedTo)
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.AssignedTo)
                .Where(t => t.AssignedToId == userId)
                .ToListAsync();
        }
        
        public async Task<TaskItem> GetTaskWithDetailsAsync(int taskId)
        {
            return await _context.Tasks
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }
    }
}