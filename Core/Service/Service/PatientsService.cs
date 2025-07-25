using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using ServiceAbstraction;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class PatientsService(IUnitOfWork unitOfWork, IMapper mapper) : IPatientsService
    {
        #region Patients
        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await unitOfWork.GetRepository<Patient, int>().GetAllAsync();
            return mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<PatientDto?> GetPatientByIdAsync(int id)
        {
            var patient = await unitOfWork.GetRepository<Patient, int>().GetByIdAsync(id);
            // AuditLog: قراءة بيانات مريض (اختياري)
            // يمكن إضافة تسجيل قراءة إذا أردت تتبع كل العمليات
            return patient == null ? null : mapper.Map<PatientDto>(patient);
        }

        // مثال: إضافة مريض جديد مع تسجيل AuditLog
        public async Task<PatientDto> AddPatientAsync(PatientDto patientDto)
        {
            var patient = mapper.Map<Patient>(patientDto);
            await unitOfWork.GetRepository<Patient, int>().AddAsync(patient);
            await unitOfWork.SaveChangesAsync();
            var audit = new AuditLog
            {
                EntityName = "Patient",
                EntityId = patient.Id,
                Action = "Created",
                ChangedBy = patient.Phone,
                ChangedAt = DateTime.Now,
                Changes = $"Created patient: {patient.Name}, {patient.Phone}, {patient.Email}, {patient.Address}"
            };
            await unitOfWork.GetRepository<AuditLog, int>().AddAsync(audit);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<PatientDto>(patient);
        }

        // مثال: تعديل بيانات مريض مع تسجيل AuditLog
        public async Task<bool> UpdatePatientAsync(int id, PatientDto patientDto)
        {
            var patient = await unitOfWork.GetRepository<Patient, int>().GetByIdAsync(id);
            if (patient == null) return false;
            var oldPatient = new { patient.Name, patient.Phone, patient.Email };
            patient.Name = patientDto.Name ?? patient.Name;
            patient.Phone = patientDto.Phone ?? patient.Phone;
            patient.Email = patientDto.Email ?? patient.Email;
            unitOfWork.GetRepository<Patient, int>().Update(patient);
            await unitOfWork.SaveChangesAsync();
            var audit = new AuditLog
            {
                EntityName = "Patient",
                EntityId = patient.Id,
                Action = "Updated",
                ChangedBy = patient.Phone,
                ChangedAt = DateTime.Now,
                Changes = $"Patient: {oldPatient} => {{Name: {patient.Name}, Phone: {patient.Phone}, Email: {patient.Email}}}"
            };
            await unitOfWork.GetRepository<AuditLog, int>().AddAsync(audit);
            await unitOfWork.SaveChangesAsync();
            return true;
        }


        public async Task<int> CountPatientsAsync()
        {
            var patients = await unitOfWork.GetRepository<Patient, int>().GetAllAsync();
            return patients.Count();
        }

        public async Task<int> CountNewPatientsThisMonthAsync()
        {
            var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var patients = await unitOfWork.GetRepository<Patient, int>().GetAllAsync();
            return patients.Count(p => p.CreatedAt >= monthStart);
        }


        #endregion

        #region Search Methods
        public async Task<IEnumerable<PatientDto>> SearchPatientsByPhoneAsync(string phoneNumber)
        {
            var patients = await unitOfWork.GetRepository<Patient, int>().GetAllAsync();
            var filteredPatients = patients.Where(p => p.Phone.Contains(phoneNumber));
            return mapper.Map<IEnumerable<PatientDto>>(filteredPatients);
        }

        public async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return Enumerable.Empty<PatientDto>();

            var repo = unitOfWork.GetRepository<Patient, int>();
            var patients = await repo.GetAllAsync();

            // تطابق كامل
            var exactMatches = patients.Where(p =>
                p.Id.ToString() == search ||
                p.Name == search ||
                p.Phone == search
            );

            // بحث جزئي
            var partialMatches = patients.Where(p =>
                p.Name.Contains(search) ||
                p.Phone.Contains(search) ||
                p.Id.ToString().Contains(search)
            );

            var results = exactMatches.Union(partialMatches).ToList();
            return mapper.Map<IEnumerable<PatientDto>>(results);
        }
        #endregion

    }
}
