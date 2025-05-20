using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data.Repositories;
using backend.Models;
using backend.Models.DTOs;

namespace backend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        
        public ProjectService(IProjectRepository projectRepository, IUserRepository userRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }
        
        public async Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(int userId)
        {
            var projects = await _projectRepository.GetUserProjectsAsync(userId);
            
            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                Deadline = p.Deadline,
                Owner = new UserDto
                {
                    Id = p.Owner.Id,
                    Username = p.Owner.Username,
                    Email = p.Owner.Email,
                    FirstName = p.Owner.FirstName,
                    LastName = p.Owner.LastName
                }
            });
        }
        
        public async Task<ProjectDto> GetProjectByIdAsync(int projectId)
        {
            var project = await _projectRepository.GetProjectWithDetailsAsync(projectId);
            
            if (project == null)
                return null;
                
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                Deadline = project.Deadline,
                Owner = new UserDto
                {
                    Id = project.Owner.Id,
                    Username = project.Owner.Username,
                    Email = project.Owner.Email,
                    FirstName = project.Owner.FirstName,
                    LastName = project.Owner.LastName
                },
                Members = project.Members.Select(m => new UserDto
                {
                    Id = m.Id,
                    Username = m.Username,
                    Email = m.Email,
                    FirstName = m.FirstName,
                    LastName = m.LastName
                }).ToList()
            };
        }
        
        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto projectDto, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
                return null;
                
            var project = new Project
            {
                Name = projectDto.Name,
                Description = projectDto.Description,
                CreatedAt = DateTime.Now,
                Deadline = projectDto.Deadline,
                OwnerId = userId,
                Members = new List<User> { user }
            };
            
            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();
            
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                Deadline = project.Deadline,
                Owner = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                },
                Members = new List<UserDto>
                {
                    new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    }
                }
            };
        }
        
        public async Task<ProjectDto> UpdateProjectAsync(int projectId, UpdateProjectDto projectDto)
        {
            var project = await _projectRepository.GetProjectWithDetailsAsync(projectId);
            
            if (project == null)
                return null;
                
            project.Name = projectDto.Name ?? project.Name;
            project.Description = projectDto.Description ?? project.Description;
            project.Deadline = projectDto.Deadline ?? project.Deadline;
            
            _projectRepository.Update(project);
            await _projectRepository.SaveChangesAsync();
            
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                Deadline = project.Deadline,
                Owner = new UserDto
                {
                    Id = project.Owner.Id,
                    Username = project.Owner.Username,
                    Email = project.Owner.Email,
                    FirstName = project.Owner.FirstName,
                    LastName = project.Owner.LastName
                },
                Members = project.Members.Select(m => new UserDto
                {
                    Id = m.Id,
                    Username = m.Username,
                    Email = m.Email,
                    FirstName = m.FirstName,
                    LastName = m.LastName
                }).ToList()
            };
        }
        
        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            
            if (project == null)
                return false;
                
            _projectRepository.Delete(project);
            return await _projectRepository.SaveChangesAsync();
        }
        
        public async Task<bool> AddUserToProjectAsync(int projectId, int userId)
        {
            return await _projectRepository.AddUserToProjectAsync(projectId, userId);
        }
        
        public async Task<bool> RemoveUserFromProjectAsync(int projectId, int userId)
        {
            return await _projectRepository.RemoveUserFromProjectAsync(projectId, userId);
        }
        
        public async Task<bool> IsUserProjectOwnerAsync(int projectId, int userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            return project != null && project.OwnerId == userId;
        }
        
        public async Task<bool> IsUserProjectMemberAsync(int projectId, int userId)
        {
            var project = await _projectRepository.GetProjectWithDetailsAsync(projectId);
            return project != null && (project.OwnerId == userId || project.Members.Any(m => m.Id == userId));
        }
    }
}