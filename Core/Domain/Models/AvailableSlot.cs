using System;

namespace Domain.Models
{
    public class AvailableSlot
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
    }
} 