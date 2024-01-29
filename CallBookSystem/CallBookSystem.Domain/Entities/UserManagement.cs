using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Domain.Entities
{
    public class UserManagement
    {
		public string Id { get; set; }
		public string UserId { get; set; }
		public string EmpName { get; set; }
		public string EmpId { get; set; }
		public string ProcessId { get; set; }
		public string ProcessName { get; set; }
		public string IsAdmin { get; set; }
		public string EntryBy { get; set; }
		public string EntryDate { get; set; }
		public string UpdatedBy { get; set; }
		public string UpdatedDate { get; set; }
	}
}
