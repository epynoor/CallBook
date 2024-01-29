using CallBookSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Application.Interfaces
{
    public interface ICallPlanRepository
    {
        Task<int> SaveAsync(string status, string userId, string actionType);
        Task<IReadOnlyList<CallPlan>> GetListAsync(CallPlan callPlan, string actionType, int pagesize = 0, int skip = 0);
        Task<IReadOnlyList<CallPlan>> GetListByTeamIdAsync(CallPlan callPlan,string teamId, string actionType);
        Task<CallPlan> GetByIdAsync(string id);

        Task<int> SaveTempAsync(CallPlan callPlan, string actionType);
        //Task<int> DeletePlanAsync(CallPlan callPlan, string actionType);
        Task<int> UpdateCallPlan(string status,CallPlan callPlan, string actionType);
        Task<int> ApproveCallPlan(string status,string id, string userId,string isAccepted, string actionType);
        Task<int> CallPlanStatusChange(string status, string id, string userId, string remarks, string actionType);
        Task<IReadOnlyList<CallPlan>> GetTempListByEmpPinAsync(string empPin, string actionType);
        Task<IReadOnlyList<CallPlan>> GetUpdateListByBatchAsync(string empPin, string actionType);
        Task<CallType> GetCustomerNameByCifId(string stringCif);

    }
}
