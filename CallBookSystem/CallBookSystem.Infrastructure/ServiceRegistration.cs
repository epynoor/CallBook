using CallBookSystem.Application.Interfaces;
using CallBookSystem.Domain.Enums;
using CallBookSystem.Infrastructure.Caching;
using CallBookSystem.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            #region Caching

            
            services.AddTransient<MemoryCacheService>();
            services.AddTransient<RedisCacheService>();
            services.AddTransient<Func<CacheTech, ICacheService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case CacheTech.Memory:
                        return serviceProvider.GetService<MemoryCacheService>();
                    case CacheTech.Redis:
                        return serviceProvider.GetService<RedisCacheService>();
                    default:
                        return serviceProvider.GetService<MemoryCacheService>();
                }
            });


            #endregion

            services.AddTransient<ISysActivityLogRepository, SysActivityLogRepository>();
            services.AddTransient<IHrUserRepository, HrUserRepository>();
            services.AddTransient<IProcessRepository, ProcessRepository>();
            services.AddTransient<ICallTypeRepository,CallTypeRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IUserManagementRepository, UserManagementRepository>();
            services.AddTransient<ICallPlanRepository, CallPlanRepository>();
            services.AddTransient<ICallReportRepository, CallReportRepository>();
            services.AddTransient<ICallReportContactPersonRepository, CallReportContactPersonRepository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
