using CallBookSystem.Domain.Entities;

namespace CallBookSystem.Web.ViewModels
{
    public class CallScheduleViewModel
    {
        public IList<CallReport> FollowupCalls { get; set; }
        public IList<CallPlan> CallPlans { get; set; }
    }
}
