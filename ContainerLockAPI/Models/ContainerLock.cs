using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContainerLockAPI.Models
{
    public class ContainerLock
    {
        [Key]
        public string ContainerId { get; set; }

        [Required]
        public string Username { get; set; } 

        [Required]
        public DateTime LockTimestamp { get; set; } = DateTime.UtcNow; 

        public string Status { get; set; }
    }
}
