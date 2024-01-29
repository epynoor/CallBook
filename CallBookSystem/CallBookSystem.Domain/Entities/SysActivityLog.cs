using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Domain.Entities
{
	public class SysActivityLog
	{
		public string Id { get; set; }
		public string ActivityLog { get; set; }
		public string CreatedTime { get; set; }
		public string ProcessName { get; set; }
		public string SurveyId { get; set; }
		public string EmpId { get; set; }
	}
}
