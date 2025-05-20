using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data.Repositories;
using backend.Models;
using backend.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace backend.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<TaskService> _logger;
        
        public TaskService(
            ITaskRepository taskRepository, 
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _logger = logger;
        }
        
        public async Task<IEnumerable<TaskDto>> GetProjectTasksAsync(int projectId)
        {
            try
            {
                _logger.LogInformation("Getting tasks for project {ProjectId} from repository", projectId);
                var tasks = await _taskRepository.GetTasksByProjectIdAsync(projectId);
                
                var taskDtos = tasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    Deadline = t.Deadline,
                    ProjectId = t.ProjectId,
                    AssignedTo = t.AssignedTo != null ? new UserDto
                    {
                        Id = t.AssignedTo.Id,
                        Username = t.AssignedTo.Username,
                        Email = t.AssignedTo.Email,
                        FirstName = t.AssignedTo.FirstName,
                        LastName = t.AssignedTo.LastName
                    } : null
                }).ToList();
                
                _logger.LogInformation("Returning {Count} tasks for project {ProjectId}", taskDtos.Count, projectId);
                return taskDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for project {ProjectId}", projectId);
                throw;
            }
        }
        
        public async Task<IEnumerable<TaskDto>> GetUserTasksAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Getting tasks for user {UserId}", userId);
                var tasks = await _taskRepository.GetUserTasksAsync(userId);
                
                return tasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    Deadline = t.Deadline,
                    ProjectId = t.ProjectId,
                    AssignedTo = t.AssignedTo != null ? new UserDto
                    {
                        Id = t.AssignedTo.Id,
                        Username = t.AssignedTo.Username,
                        Email = t.AssignedTo.Email,
                        FirstName = t.AssignedTo.FirstName,
                        LastName = t.AssignedTo.LastName
                    } : null
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for user {UserId}", userId);
                throw;
            }
        }
        
        public async Task<TaskDto> GetTaskByIdAsync(int taskId)
        {
            try
            {
                _logger.LogInformation("Getting task {TaskId}", taskId);
                var task = await _taskRepository.GetTaskWithDetailsAsync(taskId);
                
                if (task == null)
                {
                    _logger.LogWarning("Task {TaskId} not found", taskId);
                    return null;
                }
                    
                return new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    CreatedAt = task.CreatedAt,
                    Deadline = task.Deadline,
                    ProjectId = task.ProjectId,
                    AssignedTo = task.AssignedTo != null ? new UserDto
                    {
                        Id = task.AssignedTo.Id,
                        Username = task.AssignedTo.Username,
                        Email = task.AssignedTo.Email,
                        FirstName = task.AssignedTo.FirstName,
                        LastName = task.AssignedTo.LastName
                    } : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task {TaskId}", taskId);
                throw;
            }
        }
        
        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto taskDto)
        {
            try
            {
                _logger.LogInformation("Creating task for project {ProjectId}", taskDto.ProjectId);
                var project = await _projectRepository.GetByIdAsync(taskDto.ProjectId);
                
                if (project == null)
                {
                    _logger.LogWarning("Project {ProjectId} not found", taskDto.ProjectId);
                    return null;
                }
                    
                User assignedUser = null;
                if (taskDto.AssignedToId.HasValue)
                {
                    assignedUser = await _userRepository.GetByIdAsync(taskDto.AssignedToId.Value);
                    if (assignedUser == null)
                    {
                        _logger.LogWarning("User {UserId} not found", taskDto.AssignedToId.Value);
                        return null;
                    }
                }
                
                var task = new TaskItem
                {
                    Title = taskDto.Title,
                    Description = taskDto.Description,
                    Status = TaskStatusEnum.ToDo,
                    Priority = taskDto.Priority,
                    CreatedAt = DateTime.Now,
                    Deadline = taskDto.Deadline,
                    ProjectId = taskDto.ProjectId,
                    AssignedToId = taskDto.AssignedToId
                };
                
                await _taskRepository.AddAsync(task);
                await _taskRepository.SaveChangesAsync();
                
                _logger.LogInformation("Task {TaskId} created successfully", task.Id);
                
                return new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    CreatedAt = task.CreatedAt,
                    Deadline = task.Deadline,
                    ProjectId = task.ProjectId,
                    AssignedTo = assignedUser != null ? new UserDto
                    {
                        Id = assignedUser.Id,
                        Username = assignedUser.Username,
                        Email = assignedUser.Email,
                        FirstName = assignedUser.FirstName,
                        LastName = assignedUser.LastName
                    } : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task for project {ProjectId}", taskDto.ProjectId);
                throw;
            }
        }
        
        public async Task<TaskDto> UpdateTaskAsync(int taskId, UpdateTaskDto taskDto)
        {
            try
            {
                _logger.LogInformation("Updating task {TaskId}", taskId);
                var task = await _taskRepository.GetTaskWithDetailsAsync(taskId);
                
                if (task == null)
                {
                    _logger.LogWarning("Task {TaskId} not found", taskId);
                    return null;
                }
                    
                User assignedUser = null;
                if (taskDto.AssignedToId.HasValue && taskDto.AssignedToId != task.AssignedToId)
                {
                    assignedUser = await _userRepository.GetByIdAsync(taskDto.AssignedToId.Value);
                    if (assignedUser == null)
                    {
                        _logger.LogWarning("User {UserId} not found", taskDto.AssignedToId.Value);
                        return null;
                    }
                        
                    task.AssignedToId = taskDto.AssignedToId;
                }
                else if (task.AssignedToId.HasValue)
                {
                    assignedUser = task.AssignedTo;
                }
                
                task.Title = taskDto.Title ?? task.Title;
                task.Description = taskDto.Description ?? task.Description;
                task.Status = taskDto.Status;
                task.Priority = taskDto.Priority;
                task.Deadline = taskDto.Deadline ?? task.Deadline;
                
                _taskRepository.Update(task);
                await _taskRepository.SaveChangesAsync();
                
                _logger.LogInformation("Task {TaskId} updated successfully", taskId);
                
                return new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    CreatedAt = task.CreatedAt,
                    Deadline = task.Deadline,
                    ProjectId = task.ProjectId,
                    AssignedTo = assignedUser != null ? new UserDto
                    {
                        Id = assignedUser.Id,
                        Username = assignedUser.Username,
                        Email = assignedUser.Email,
                        FirstName = assignedUser.FirstName,
                        LastName = assignedUser.LastName
                    } : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", taskId);
                throw;
            }
        }
        
        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            try
            {
                _logger.LogInformation("Deleting task {TaskId}", taskId);
                var task = await _taskRepository.GetByIdAsync(taskId);
                
                if (task == null)
                {
                    _logger.LogWarning("Task {TaskId} not found", taskId);
                    return false;
                }
                    
                _taskRepository.Delete(task);
                var result = await _taskRepository.SaveChangesAsync();
                
                if (result)
                {
                    _logger.LogInformation("Task {TaskId} deleted successfully", taskId);
                }
                else
                {
                    _logger.LogWarning("Failed to delete task {TaskId}", taskId);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", taskId);
                throw;
            }
        }
    }
}