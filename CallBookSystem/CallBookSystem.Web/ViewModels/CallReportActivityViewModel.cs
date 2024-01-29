using CallBookSystem.Domain.Entities;

namespace CallBookSystem.Web.ViewModels
{
    public class CallReportActivityViewModel
    {
        public CallReportActivity ReportActivity { get; set; }

        public IList<CallReportContactPerson> ContactPersons { get; set; }

        public IList<CallingOfficer> CallingOfficers { get; set; }

        
        public string ActionType { get; set; }
    }
}
