using Domain.Models;
using Shared.Dtos;

namespace ServiceAbstraction
{
    public interface IMedicalCaseService
    {
        Task<MedicalCaseDto> CreateMedicalCaseAsync(CreateMedicalCaseDto dto);
        Task<MedicalCaseDto> UpdateMedicalCaseAsync(int id, UpdateMedicalCaseDto dto);
        Task<MedicalCaseDto> GetMedicalCaseByIdAsync(int id);
        Task<List<MedicalCaseDto>> GetAllMedicalCasesAsync();
        Task<bool> DeleteMedicalCaseAsync(int id);  
    }
} 