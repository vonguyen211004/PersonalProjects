using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }
        
        public DateTime? Deadline { get; set; }
        
        [ForeignKey("Owner")]
        public int OwnerId { get; set; }
        
        // Navigation properties
        public User Owner { get; set; }
        public ICollection<TaskItem> Tasks { get; set; }
        public ICollection<User> Members { get; set; }
    }
}