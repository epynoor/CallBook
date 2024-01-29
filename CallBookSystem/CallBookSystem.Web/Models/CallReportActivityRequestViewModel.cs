namespace CallBookSystem.Web.Models
{
    public class CallReportActivityRequestViewModel
    {
        public string ActivityId { get; set; }
        public string CallId { get; set; }
        public string EmpPin { get;set; }
        public string ProcessId { get;set; }


        public string PageNumber { get; set; }
        public string PageSize { get; set; }
        public string Type { get; set; }
    }
}
