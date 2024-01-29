using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Domain.Entities
{
    public class HrUser
    {        
        public string Id { get; set; }
        
        public string EmpId { get; set; }

        public string EmpName { get; set; }

        public string EmailId { get; set; }

        public string MobileNo { get; set; }

        public string DepartmentId { get; set; }

        public string Department { get; set; }

        public string DesignationId { get; set; }

        public string Designation { get; set; }

        public string LocationId { get; set; }

        public string Location { get; set; }

        public string UserId { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public string IsOneTimePassword { get; set; }

        public string JoiningDate { get; set; }

        public string IsActive { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public string UpdatedDate { get; set; }

        public string DivisionId { get; set; }

        public string Division { get; set; }

        public string JobId { get; set; }

        public string JobName { get; set; }

        public string GradeId { get; set; }

        public string GradeName { get; set; }

        public string PositionId { get; set; }

        public string PositionName { get; set; }

        public string SolCode { get; set; }

        public string SolName { get; set; }

        public string OrganizationId { get; set; }

        public string UnitId { get; set; }

        public string UnitName { get; set; }

        public string SuperVisorId { get; set; }

        public string SuperVisorName { get; set; }

        public string JobStatus { get; set; }

        public string BloodGroup { get; set; }

        public string ProcessId { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsLineManager { get; set; }
    }
}
