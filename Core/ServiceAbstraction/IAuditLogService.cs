using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(string? entity, string? action, DateTime? from, DateTime? to, string? search = null);
    }
} 