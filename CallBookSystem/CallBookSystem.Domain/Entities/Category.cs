using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Domain.Entities
{
    public class Category
    {
		public string Id { get; set; }
		public string CategoryName { get; set; }
		public string ProcessId { get; set; }
		public string ProcessName { get; set; }
		public string EntryBy { get; set; }
		public string EntryDate { get; set; }
		public string UpdatedBy { get; set; }
		public string UpdatedDate { get; set; }
	}
}
