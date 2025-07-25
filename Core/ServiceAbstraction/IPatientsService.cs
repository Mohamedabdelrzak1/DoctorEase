using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
   public interface IPatientsService
    {
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto?> GetPatientByIdAsync(int id);
        Task<IEnumerable<PatientDto>> SearchPatientsByPhoneAsync(string phoneNumber);
        Task<IEnumerable<PatientDto>> SearchPatientsAsync(string search);
        Task<int> CountPatientsAsync();
        Task<int> CountNewPatientsThisMonthAsync();
    }
}
