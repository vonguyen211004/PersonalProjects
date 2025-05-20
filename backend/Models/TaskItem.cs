using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        [Required]
        public TaskStatusEnum Status { get; set; }
        
        [Required]
        public TaskPriority Priority { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }
        
        public DateTime? Deadline { get; set; }
        
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        
        [ForeignKey("AssignedTo")]
        public int? AssignedToId { get; set; }
        
        // Navigation properties
        public Project Project { get; set; }
        public User AssignedTo { get; set; }
    }
    
    public enum TaskStatusEnum
    {
        ToDo,
        InProgress,
        Review,
        Done
    }
    
    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}