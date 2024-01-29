using CallBookSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Application.Interfaces
{
    public interface ICallReportRepository
    {

        Task<IReadOnlyList<CallReport>> GetCallReportAsync(string empPin, string processId, string callId,
            string pageSize, string skip, string actionType);
        Task<IReadOnlyList<CallReport>> GetCallReportAsyncByTeam(string empPin,string teamId, string processId, string callId,
            string pageSize, string skip, string actionType);
        Task<IList<CallingOfficer>> GetCallingOfficerAsync(string callId);
        Task<Tuple<string, string>> SaveCallReportAsync(CallReportActivity activity, IList<CallReportContactPerson> contactPeople, IList<CallingOfficer> callingOfficers, string actionType);
        Task<bool> SaveUploadedFile(IList<CallReportFile> callReportFiles, string userName);
        Task<IReadOnlyList<CallReportActivity>> GetCallActivityDetailsAsync(CallReportActivity callActvitydetails, string actionType);
        Task<IReadOnlyList<CallReport>> GetFollowupCallScheduleAsync(string empPin, string processId);
        Task<IReadOnlyList<ChartData>> GetChartData(string firstDate, string lastDate, string actionType);

    }
}
