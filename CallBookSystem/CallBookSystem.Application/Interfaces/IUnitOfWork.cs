namespace CallBookSystem.Application.Interfaces
{
    public interface IUnitOfWork
    {
        ISysActivityLogRepository LogRepository { get; }
        IHrUserRepository HrUserRepository { get; }
        IProcessRepository ProcessRepository { get; }
        ICallTypeRepository CallTypeRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IUserManagementRepository UserManagementRepository { get; }
        ICallPlanRepository CallPlanRepository { get; }
        ICallReportRepository CallReportRepository { get; }
        ICallReportContactPersonRepository CallReportContactPersonRepository { get; }

    }
}
