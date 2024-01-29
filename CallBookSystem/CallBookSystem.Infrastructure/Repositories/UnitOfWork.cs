using CallBookSystem.Application.Interfaces;

namespace CallBookSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IHrUserRepository hrUserRepository, ISysActivityLogRepository logRepository, IProcessRepository processRepository, ICallTypeRepository callTypeRepository
            , ICategoryRepository categoryRepository,IUserManagementRepository userManagementRepository, ICallPlanRepository callPlanRepository
            , ICallReportRepository callReportActivityRepository, ICallReportContactPersonRepository callReportContactPersonRepository)
        {
            this.HrUserRepository = hrUserRepository;
            this.LogRepository = logRepository;
            this.ProcessRepository = processRepository;
            this.CallTypeRepository = callTypeRepository;
            this.CategoryRepository = categoryRepository;
            this.UserManagementRepository = userManagementRepository;
            this.CallPlanRepository = callPlanRepository;
            this.CallReportRepository = callReportActivityRepository;
            this.CallReportContactPersonRepository = callReportContactPersonRepository;
        }

        public IHrUserRepository HrUserRepository { get; }
        public ISysActivityLogRepository LogRepository { get; } 
        public IProcessRepository ProcessRepository { get; }
        public ICallTypeRepository CallTypeRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public ICallPlanRepository CallPlanRepository { get; }

        public IUserManagementRepository UserManagementRepository { get; }

        public ICallReportRepository CallReportRepository { get; }
        public ICallReportContactPersonRepository CallReportContactPersonRepository { get; }
    }
}
