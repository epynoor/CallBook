using CallBookSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Application.Interfaces
{
    public interface IProcessRepository
    {
        Task<IReadOnlyList<Process>> GetAllProcessesAsync();
        Task<Process> GetProcessAsync(string id, string processName, string actionType);
        Task<int> SaveProcessAsync(Process process, string userId, string actionType);
    }
}
