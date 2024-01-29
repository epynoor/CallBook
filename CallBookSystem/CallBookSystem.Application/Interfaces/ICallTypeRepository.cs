using CallBookSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Application.Interfaces
{
    public interface ICallTypeRepository
    {
        Task<IReadOnlyList<CallType>> GetAllTypeAsync();
        Task<CallType> GetTypeAsync(string id, string processId, string typeName, string actionType);
        Task<int> SaveType(CallType type, string userId, string actionType);

    }
}
