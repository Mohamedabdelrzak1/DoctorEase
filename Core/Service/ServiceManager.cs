using AutoMapper;
using Domain.Contracts;
using Service.Service;
using ServiceAbstraction;
using System;
using Microsoft.Extensions.Options;
using Shared.Dtos.Auth;
using Service.Helpers;

namespace Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly IMailingService _mailingService;
        private readonly Lazy<IBookingService> _lazyBookingService;
        private readonly Lazy<IArticlesService> _lazyArticlesService;
        private readonly Lazy<IPatientsService> _lazyPatientsService;
        private readonly Lazy<ITestimonialsService> _lazyTestimonialsService;
        private readonly Lazy<IAuditLogService> _lazyAuditLogService;
        private readonly Lazy<IAuthService> _lazyAuthService;
        private readonly Lazy<IMedicalCaseService> _lazyMedicalCaseService;


        public ServiceManager(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IOptions<JwtOptions> jwtOptions,
            IMailingService mailingService
          
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtOptions = jwtOptions;
            _mailingService = mailingService;
 
            _lazyBookingService = new Lazy<IBookingService>(() => new BookingService(_unitOfWork, _mapper, _mailingService));
            _lazyArticlesService = new Lazy<IArticlesService>(() => new ArticlesService(_unitOfWork, _mapper));
            _lazyPatientsService = new Lazy<IPatientsService>(() => new PatientsService(_unitOfWork, _mapper));
            _lazyTestimonialsService = new Lazy<ITestimonialsService>(() => new TestimonialsService(_unitOfWork, _mapper));
            _lazyAuditLogService = new Lazy<IAuditLogService>(() => new AuditLogService(_unitOfWork, _mapper));
            _lazyAuthService = new Lazy<IAuthService>(() => new AuthService(_unitOfWork, _jwtOptions));
            _lazyMedicalCaseService = new Lazy<IMedicalCaseService>(() => new MedicalCaseService(_unitOfWork));
        }

        public IBookingService BookingService => _lazyBookingService.Value;
        public IArticlesService ArticlesService => _lazyArticlesService.Value;
        public IPatientsService PatientsService => _lazyPatientsService.Value;
        public ITestimonialsService TestimonialsService => _lazyTestimonialsService.Value;
        public IAuditLogService AuditLogService => _lazyAuditLogService.Value;
        public IAuthService AuthService => _lazyAuthService.Value;
        public IMedicalCaseService MedicalCaseService => _lazyMedicalCaseService.Value;
       

        public IUnitOfWork UnitOfWork => _unitOfWork;

    }
}
