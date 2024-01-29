using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Domain.Entities
{
    public class CallPlan
    {
        public long Id { get; set; }

        public string BatchNo { get; set; }

        public string CallPlanId { get; set; }

        public string EmpPin { get; set; }

        public int ProcessId { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int CallTypeId { get; set; }

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

        public string cif { get; set; }

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

        public string EmpName { get; set; }
        public string SuperVisorId { get; set; }

        public string Purpose { get; set; }

        public DateTime TentativeDate { get; set; }
        public string strTentativeDate { get; set; }

        public string Status { get; set; }

        public string IsAccepted { get; set; }

        public string ApprovedBy { get; set; }

        public DateTime ApprovedDate { get; set; }

        public string ApprovedRemarks { get; set; }

        public string EntryBy { get; set; }

        public DateTime EntryDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string NumOfCall { get; set; }
        public string Remarks { get; set; }

        public int TotalRow { get; set; }
        public string acct_name { get; set; }
        
    }
}
