using System;

namespace Shared.Dtos
{
    public class AuditLogDto
    {
        public string EntityName { get; set; } = null!;
        public int EntityId { get; set; }
        public string Action { get; set; } = null!;
        public string? ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? Changes { get; set; }
    }
} 