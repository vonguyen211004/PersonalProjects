using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public byte[] PasswordHash { get; set; }
        
        [Required]
        public byte[] PasswordSalt { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        // Navigation properties
        public ICollection<Project> OwnedProjects { get; set; }
        public ICollection<TaskItem> AssignedTasks { get; set; }
        public ICollection<Project> Projects { get; set; }
    }
}