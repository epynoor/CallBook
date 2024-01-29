using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Domain.Entities
{
    public class CallType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string EntryBy { get; set; }
        public string EntryDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string acct_name { get; set; }
    }
}
