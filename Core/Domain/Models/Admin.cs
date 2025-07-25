using System;

namespace Domain.Models
{
    public class Admin : BaseEntity<int>
    {
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 