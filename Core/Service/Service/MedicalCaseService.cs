using Domain.Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using ServiceAbstraction;
using Shared.Dtos;
using Service.Helpers; // ✅ إضافة لاستخدام ImageSettings

namespace Service.Service
{
    public class MedicalCaseService : IMedicalCaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const string _folderName = "medical-cases"; // 📌 ثابت لمجلد الصور

        public MedicalCaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MedicalCaseDto> CreateMedicalCaseAsync(CreateMedicalCaseDto dto)
        {
            if (dto.BeforeImage == null || dto.BeforeImage.Length == 0)
                throw new ArgumentException("Before image is required");

            if (dto.AfterImage == null || dto.AfterImage.Length == 0)
                throw new ArgumentException("After image is required");

            
            var beforeImageUrl = ImageSettings.UploadImage(dto.BeforeImage, _folderName);
            var afterImageUrl = ImageSettings.UploadImage(dto.AfterImage, _folderName);

            var medicalCase = new MedicalCase
            {
                Title = dto.Title,
                BeforeImageUrl = beforeImageUrl,
                AfterImageUrl = afterImageUrl,
            };

            await _unitOfWork.GetRepository<MedicalCase, int>().AddAsync(medicalCase);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(medicalCase);
        }

        public async Task<MedicalCaseDto> UpdateMedicalCaseAsync(int id, UpdateMedicalCaseDto dto)
        {
            var medicalCase = await _unitOfWork.GetRepository<MedicalCase, int>().GetByIdAsync(id)
                ?? throw new ArgumentException("Medical case not found");

            if (!string.IsNullOrEmpty(dto.Title))
                medicalCase.Title = dto.Title;

            if (dto.BeforeImage != null)
            {
                ImageSettings.DeleteImage(medicalCase.BeforeImageUrl, _folderName);
                medicalCase.BeforeImageUrl = ImageSettings.UploadImage(dto.BeforeImage, _folderName);
            }

            if (dto.AfterImage != null)
            {
                ImageSettings.DeleteImage(medicalCase.AfterImageUrl, _folderName);
                medicalCase.AfterImageUrl = ImageSettings.UploadImage(dto.AfterImage, _folderName);
            }

            _unitOfWork.GetRepository<MedicalCase, int>().Update(medicalCase);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(medicalCase);
        }

        public async Task<MedicalCaseDto> GetMedicalCaseByIdAsync(int id)
        {
            var medicalCase = await _unitOfWork.GetRepository<MedicalCase, int>().GetByIdAsync(id);
            return medicalCase == null ? null : MapToDto(medicalCase);
        }

        public async Task<List<MedicalCaseDto>> GetAllMedicalCasesAsync()
        {
            var medicalCases = await _unitOfWork.GetRepository<MedicalCase, int>().GetAllAsync();
            return medicalCases.Select(MapToDto).ToList();
        }

        public async Task<bool> DeleteMedicalCaseAsync(int id)
        {
            var medicalCase = await _unitOfWork.GetRepository<MedicalCase, int>().GetByIdAsync(id);
            if (medicalCase == null)
                return false;

            ImageSettings.DeleteImage(medicalCase.BeforeImageUrl, _folderName);
            ImageSettings.DeleteImage(medicalCase.AfterImageUrl, _folderName);

            _unitOfWork.GetRepository<MedicalCase, int>().Delete(medicalCase);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        #region Helpers
        private static MedicalCaseDto MapToDto(MedicalCase mc)
        {
            return new MedicalCaseDto
            {
                Id = mc.Id,
                Title = mc.Title,
                BeforeImageUrl = mc.BeforeImageUrl,
                AfterImageUrl = mc.AfterImageUrl
            };
        }
        #endregion
    }
}
