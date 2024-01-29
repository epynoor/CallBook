using CallBookSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Application.Interfaces
{
    public interface ICallReportContactPersonRepository
    {
        Task<IList<CallReportContactPerson>> GetCallReportContactPeopleAsync(string callId, string actionType);
    }
}
