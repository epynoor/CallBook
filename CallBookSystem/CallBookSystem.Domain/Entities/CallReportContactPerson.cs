using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Domain.Entities
{
    public class CallReportContactPerson
    {
        public string Id { get; set; }
        public string CallId { get; set; }
        public string ActivityId { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string Relation { get; set; }
        public string Address { get; set; }
        public string IsActive { get; set; }
        public string EntryBy { get; set; }
        public string EntryDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}
