using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Domain.Entities
{
    public class CallingOfficer
    {
		public string Id { get; set; }
		public string CallId { get; set; }
		public string EmpPin { get; set; }
		public string ActivityId { get; set; }
		public string EmpName { get; set; }
		public string EntryBy { get; set; }
		public string EntryDate { get; set; }
		public string UpdatedBy { get; set; }
		public string UpdatedDate { get; set; }
	}
}
