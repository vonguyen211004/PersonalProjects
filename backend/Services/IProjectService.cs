using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Models.DTOs;

namespace backend.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(int userId);
        Task<ProjectDto> GetProjectByIdAsync(int projectId);
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto projectDto, int userId);
        Task<ProjectDto> UpdateProjectAsync(int projectId, UpdateProjectDto projectDto);
        Task<bool> DeleteProjectAsync(int projectId);
        Task<bool> AddUserToProjectAsync(int projectId, int userId);
        Task<bool> RemoveUserFromProjectAsync(int projectId, int userId);
        Task<bool> IsUserProjectOwnerAsync(int projectId, int userId);
        Task<bool> IsUserProjectMemberAsync(int projectId, int userId);
    }
}