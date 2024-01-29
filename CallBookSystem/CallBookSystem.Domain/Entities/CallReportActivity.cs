using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Domain.Entities
{
    public class CallReportActivity
    {
        public string ActivityId { get; set; }

        public string CallId { get; set; }

        public DateTime? CallDate { get; set; }

        public string UnitId { get; set; }
        public string UnitName { get; set; }

        public string LocationId { get; set; }
        public string Location { get; set; }

        public string EmpPin { get; set; }

        public string ProcessId { get; set; }

        public string CategoryId { get; set; }
        public string CategoryName { get; set; }

        public string CallTypeId { get; set; }
        public string CallTypeName { get; set; }

        public string PlanBatchNo { get; set; }

        public string PlanCallId { get; set; }

        public string Subject { get; set; }

        public string ClientGroup { get; set; }

        public string CIF { get; set; }

        public string Name { get; set; }

        public DateTime? FirstCallDate { get; set; }

        public string Summary { get; set; }

        public string ContactPerson { get; set; }

        public string ActionPlan { get; set; }

        public DateTime? NextFollowUpDate { get; set; }

        public string FollowUpNote { get; set; }

        public string Status { get; set; }

        public string EntryBy { get; set; }

        public DateTime? EntryDate { get; set; }

        public int TotalRow { get; set; }
        public string FileName { get; set; }
        public string Address { get; set; }
    }
}
