using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Domain.Entities
{
    public class CallReportFile
    {
        public string Id { get; set; }
        public string ProcessId { get; set; }
        public string CallId { get; set; }
        public string ActivityId { get; set; }
        public string FileName { get; set; }
        public string FileLocation { get; set; }
        public string EntryBy { get; set; }
        public string EntryDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}
