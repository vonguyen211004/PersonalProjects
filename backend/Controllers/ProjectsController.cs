using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.Models.DTOs;
using backend.Services;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        
        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUserProjects()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var projects = await _projectService.GetUserProjectsAsync(userId);
            
            return Ok(projects);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if (!await _projectService.IsUserProjectMemberAsync(id, userId))
                return Unauthorized();
                
            var project = await _projectService.GetProjectByIdAsync(id);
            
            if (project == null)
                return NotFound();
                
            return Ok(project);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateProject(CreateProjectDto projectDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var project = await _projectService.CreateProjectAsync(projectDto, userId);
            
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, UpdateProjectDto projectDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if (!await _projectService.IsUserProjectOwnerAsync(id, userId))
                return Unauthorized();
                
            var project = await _projectService.UpdateProjectAsync(id, projectDto);
            
            if (project == null)
                return NotFound();
                
            return Ok(project);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if (!await _projectService.IsUserProjectOwnerAsync(id, userId))
                return Unauthorized();
                
            var result = await _projectService.DeleteProjectAsync(id);
            
            if (!result)
                return NotFound();
                
            return NoContent();
        }
        
        [HttpPost("{id}/members/{memberId}")]
        public async Task<IActionResult> AddMember(int id, int memberId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if (!await _projectService.IsUserProjectOwnerAsync(id, userId))
                return Unauthorized();
                
            var result = await _projectService.AddUserToProjectAsync(id, memberId);
            
            if (!result)
                return BadRequest("Failed to add member to project");
                
            return NoContent();
        }
        
        [HttpDelete("{id}/members/{memberId}")]
        public async Task<IActionResult> RemoveMember(int id, int memberId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if (!await _projectService.IsUserProjectOwnerAsync(id, userId))
                return Unauthorized();
                
            var result = await _projectService.RemoveUserFromProjectAsync(id, memberId);
            
            if (!result)
                return BadRequest("Failed to remove member from project");
                
            return NoContent();
        }
    }
}