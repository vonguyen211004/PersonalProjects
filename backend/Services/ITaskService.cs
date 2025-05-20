using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Models.DTOs;

namespace backend.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetProjectTasksAsync(int projectId);
        Task<IEnumerable<TaskDto>> GetUserTasksAsync(int userId);
        Task<TaskDto> GetTaskByIdAsync(int taskId);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto taskDto);
        Task<TaskDto> UpdateTaskAsync(int taskId, UpdateTaskDto taskDto);
        Task<bool> DeleteTaskAsync(int taskId);
    }
}