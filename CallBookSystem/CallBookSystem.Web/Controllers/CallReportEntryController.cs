using CallBookSystem.Application.Interfaces;
using CallBookSystem.Domain.Entities;
using CallBookSystem.Domain.Enums;
using CallBookSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CallBookSystem.Web.Controllers
{
    [Authorize]
    public class CallReportEntryController : BaseController
    {
        private readonly ILogger<CallReportEntryController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private IConfiguration _configuration;
        private SysActivityLog log;

        public CallReportEntryController(ILogger<CallReportEntryController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            log = new SysActivityLog();
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCallReport(string callId)
        {
            try
            {
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string message = "Sorry!!! No Data Found!!!";

                CallReport callReport = null;
                if (!string.IsNullOrEmpty(callId))
                {                    
                    string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                    callReport = (await _unitOfWork.CallReportRepository.GetCallReportAsync(userId, processId, callId, "1", "0", ActionType.GetById.ToString())).FirstOrDefault();
                }

                if (callReport == null)
                {
                    callReport = new CallReport();
                }

                return Json(new { data = callReport, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCallingOfficerList(string callId)
        {
            try
            {
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string message = "Sorry!!! No Data Found!!!";
                var data = await _unitOfWork.CallReportRepository.GetCallingOfficerAsync(callId);

                if(data.Where(p=>p.EmpPin == userId).Count() == 0)
                {
                    data.Add(new CallingOfficer() { EmpPin = userId, EmpName = User.Identity.Name });
                }
                return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetCallReportContactPerson(string callId)
        {
            try
            {               
                var data = await _unitOfWork.CallReportContactPersonRepository.GetCallReportContactPeopleAsync(callId, ActionType.GetByCallId.ToString());
                return Json(new { data = data, status = "success",  result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public IActionResult GetCallReportStatusList()
        {
            var  data =  Enum.GetValues(typeof(CallReportStatus))
            .Cast<CallReportStatus>()
            .Select(v => v.ToString())
            .ToList();

            return Json(new { data = data, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
        }

        [HttpPost]
        public async Task<IActionResult> SaveCallReportActivity(CallReportActivityViewModel callReport)
        {
            string userName = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            callReport.ReportActivity.EmpPin = userName;
            callReport.ReportActivity.UnitId =  User.Claims.Where(c => c.Type == "UnitId").FirstOrDefault().Value;
            callReport.ReportActivity.LocationId = User.Claims.Where(c => c.Type == "LocationId").FirstOrDefault().Value;
            callReport.ReportActivity.ProcessId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
            


            try
            {
                string message = "";
                bool saved = false;

                string callId = string.Empty;
                string activityId = string.Empty;

                var callRpt =   await _unitOfWork.CallReportRepository.SaveCallReportAsync(callReport.ReportActivity, callReport.ContactPersons, callReport.CallingOfficers, callReport.ActionType);

                if(callRpt != null)
                {
                    message = "Call report saved successfully!!!!!";
                    saved = true;
                    callId = callRpt.Item1;
                    activityId = callRpt.Item2;
                }
                else
                {
                    message = "Call report save failed!!!!!";
                    saved = false;
                }

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + message;
                log.ProcessName = "CallPlan/DismissCallPlan";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);


                return Json(new { status = saved, message = message, callId = callId, activityId = activityId });
            }
            catch (Exception ex)
            {
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + ex.Message;
                log.ProcessName = "CallPlan/DismissCallPlan";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                throw;
            }
        }

        public async Task<IActionResult> getCIFName(string cif)
        {
            CallType callPlan = new CallType();
            var data = await _unitOfWork.CallPlanRepository.GetCustomerNameByCifId(cif.Trim());
            return Json(new { data = data, status = "success", result = CommonAjaxResponse("success", "success", "200") });
        }

        public async Task<IActionResult> FileUpload(string callId, string activityId)
        {
            try
            {


                var FileDic = "Files";

                string location = _configuration.GetSection("AppSettings").GetSection("AttachedFileLocation").Value;

                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;

                //string FilePath = Path.Combine(location, FileDic);


                IFormFileCollection files = Request.Form.Files;

                IList<CallReportFile> callReportFiles = new List<CallReportFile>();
                CallReportFile callReportFile = null;
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {

                        string filePath = Path.Combine(location, "C_" + callId + "_" + activityId + "_" + file.FileName);
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        callReportFile = new CallReportFile();
                        callReportFile.ActivityId = activityId; ;
                        callReportFile.FileLocation = filePath;
                        callReportFile.CallId = callId;
                        callReportFile.FileName = file.FileName;
                        callReportFile.ProcessId = processId;

                        callReportFiles.Add(callReportFile);
                    }
                }
                string userName = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                bool done = await _unitOfWork.CallReportRepository.SaveUploadedFile(callReportFiles, userName);


                if (done)
                {
                    return Json(new { status = done, message = "Saved Successfully", callId = callId, activityId = activityId });
                }
                else
                {
                    return Json(new { status = done, message = "Save Failed", callId = callId, activityId = activityId });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }        
    }
    
}

