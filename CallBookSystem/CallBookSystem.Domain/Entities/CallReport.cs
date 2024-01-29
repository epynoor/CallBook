using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Domain.Entities
{
    public class CallReport
    {
        public long CallId { get; set; }

        public string PlanBatchNo { get; set; }

        public string PlanCallId { get; set; }

        public string UnitId { get; set; }

        public string LocationId { get; set; }

        public string EmpPin { get; set; }
        public string EmpName { get; set; }

        public int? ProcessId { get; set; }

        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int? CallTypeId { get; set; }
        public string CallTypeName { get; set; }

        public string Subject { get; set; }

        public string ShortSubject
        {
            get
            {
                var text = Subject;
                if (text.Length > 28)
                {
                    text = text.Remove(26);
                    text += "..";
                }
                return text;
            }
        }

        public string ClientGroup { get; set; }

        public string CIF { get; set; }

        public string Name { get; set; }

        public string ShortName
        {
            get
            {
                var text = Name;
                if (text.Length > 25)
                {
                    text = text.Remove(23);
                    text += "..";
                }
                return text;
            }
        }

        public DateTime? FirstCallDate { get; set; }

        public DateTime? LastCallDate { get; set; }

        public DateTime? NextFollowUpDate { get; set; }
        public string strNextFollowUpDate { get; set; }

        public string FollowUpNote { get; set; }

        public string Status { get; set; }

        public string EntryBy { get; set; }

        public DateTime? EntryDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string Address { get; set; }

        public int TotalRow { get; set; }
    }
}
