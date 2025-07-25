using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using ServiceAbstraction;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Service
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AuditLogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(string? entity, string? action, DateTime? from, DateTime? to, string? search = null)
        {
            var logs = await _unitOfWork.GetRepository<AuditLog, int>().GetAllAsync();
            if (!string.IsNullOrEmpty(entity))
                logs = logs.Where(l => l.EntityName == entity);
            if (!string.IsNullOrEmpty(action))
                logs = logs.Where(l => l.Action == action);
            if (from.HasValue)
                logs = logs.Where(l => l.ChangedAt >= from.Value);
            if (to.HasValue)
                logs = logs.Where(l => l.ChangedAt <= to.Value);
            if (!string.IsNullOrEmpty(search))
                logs = logs.Where(l => (l.ChangedBy ?? "").Contains(search, StringComparison.OrdinalIgnoreCase) || (l.Changes ?? "").Contains(search, StringComparison.OrdinalIgnoreCase));
            var dtos = _mapper.Map<IEnumerable<AuditLogDto>>(logs.OrderByDescending(l => l.ChangedAt));
            return dtos;
        }
    }
} 