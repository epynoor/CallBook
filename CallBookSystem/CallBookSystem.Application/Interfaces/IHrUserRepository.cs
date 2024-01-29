using CallBookSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Application.Interfaces
{
    public interface IHrUserRepository
    {
        Task<HrUser> GetUserInfo(string userId);
        Task<IReadOnlyList<HrUser>> GetAllUser(string supervisorId);
        Task<IReadOnlyList<HrUser>> GetAllUserBySupervisorId(string supervisorId);
        Task<HrUser> GetUserInfoByUserIdAndPassword(string userId, string password);
    }
}
