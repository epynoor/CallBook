using CallBookSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Application.Interfaces
{
    public interface IUserManagementRepository
    {
        Task<IReadOnlyList<UserManagement>> GetAllUserManagementAsync();
        Task<UserManagement> GetUserManagementAsync(string id, string userId, string processId, string actionType);
        Task<int> SaveUserManagementAsync(UserManagement userManagement, string userId, string actionType);
    }
}
