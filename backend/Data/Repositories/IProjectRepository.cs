using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Models;

namespace backend.Data.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project> GetProjectWithDetailsAsync(int projectId);
        Task<IEnumerable<Project>> GetUserProjectsAsync(int userId);
        Task<bool> AddUserToProjectAsync(int projectId, int userId);
        Task<bool> RemoveUserFromProjectAsync(int projectId, int userId);
    }
}