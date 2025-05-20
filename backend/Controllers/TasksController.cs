using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.Models.DTOs;
using backend.Services;
using Microsoft.Extensions.Logging;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly ILogger<TasksController> _logger;
        
        public TasksController(
            ITaskService taskService, 
            IProjectService projectService,
            ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _projectService = projectService;
            _logger = logger;
        }
        
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetProjectTasks(int projectId)
        {
            _logger.LogInformation("Getting tasks for project {ProjectId}", projectId);
            
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _logger.LogInformation("User {UserId} requesting tasks for project {ProjectId}", userId, projectId);
            
            if (!await _projectService.IsUserProjectMemberAsync(projectId, userId))
            {
                _logger.LogWarning("User {UserId} is not a member of project {ProjectId}", userId, projectId);
                return Unauthorized();
            }
                
            var tasks = await _taskService.GetProjectTasksAsync(projectId);
            _logger.LogInformation("Found {Count} tasks for project {ProjectId}", tasks.Count(), projectId);
            
            return Ok(tasks);
        }
        
        [HttpGet("user")]
        public async Task<IActionResult> GetUserTasks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var tasks = await _taskService.GetUserTasksAsync(userId);
            
            return Ok(tasks);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            
            if (task == null)
                return NotFound();
                
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if (!await _projectService.IsUserProjectMemberAsync(task.ProjectId, userId))
                return Unauthorized();
                
            return Ok(task);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskDto taskDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if (!await _projectService.IsUserProjectMemberAsync(taskDto.ProjectId, userId))
                return Unauthorized();
                
            var task = await _taskService.CreateTaskAsync(taskDto);
            
            if (task == null)
                return BadRequest("Failed to create task");
                
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto taskDto)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            
            if (task == null)
                return NotFound();
                
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if (!await _projectService.IsUserProjectMemberAsync(task.ProjectId, userId))
                return Unauthorized();
                
            var updatedTask = await _taskService.UpdateTaskAsync(id, taskDto);
            
            if (updatedTask == null)
                return BadRequest("Failed to update task");
                
            return Ok(updatedTask);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            
            if (task == null)
                return NotFound();
                
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if (!await _projectService.IsUserProjectOwnerAsync(task.ProjectId, userId))
                return Unauthorized();
                
            var result = await _taskService.DeleteTaskAsync(id);
            
            if (!result)
                return BadRequest("Failed to delete task");
                
            return NoContent();
        }
    }
}