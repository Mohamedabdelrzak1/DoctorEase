using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IServiceManager
    {
        public IBookingService BookingService { get; }
        public IPatientsService PatientsService { get; }

        public ITestimonialsService TestimonialsService { get; }
        public IArticlesService ArticlesService { get; }
        public IAuditLogService AuditLogService { get; }
        public IAuthService AuthService { get; }
        public IMedicalCaseService MedicalCaseService { get; }

        IUnitOfWork UnitOfWork { get; }




    }
}
