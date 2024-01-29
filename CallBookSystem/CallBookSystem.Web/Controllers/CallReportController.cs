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
    public class CallReportController : BaseController
    {

        private readonly ILogger<CallReportController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private SysActivityLog log;

        public CallReportController(ILogger<CallReportController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            log = new SysActivityLog();
        }

        public async Task<IActionResult> Index()
        {
            string supervisor = string.Empty;
            ChartData callPlan = new ChartData();

            string role = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;

            if (role == AppRole.ADMIN || role == AppRole.PROCESS_ADMIN || role == AppRole.LINE_MANAGER)
            {
                supervisor = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            }
            ViewBag.Supervisor = supervisor;

           

            //TempData["EmpPin"] = data.AsEnumerable().Select(r => r.EmpPin).ToArray();


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetCallReportList()
        {
            try
            {
                int totalRecord = 0;
                int filterRecord = 0;

                var draw = Request.Form["draw"].FirstOrDefault();
                string pageSize = (Request.Form["length"].FirstOrDefault() ?? "0");
                string skip = (Request.Form["start"].FirstOrDefault() ?? "0");

                CallPlan callPlan = new CallPlan(); 
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                string message = "Sorry!!! No Data Found!!!";
                callPlan.EmpPin = userId;
                callPlan.Status = CallPlanStatus.APPROVED.ToString();
                callPlan.ProcessId = Convert.ToInt32(processId);
                callPlan.strTentativeDate = DateTime.Now.ToString("dd-MMM-yyyy");

                var data = (await _unitOfWork.CallReportRepository.GetCallReportAsync(userId, processId, string.Empty
                    , pageSize: pageSize, skip: skip, actionType: ActionType.GetCallReportByPaging.ToString())).ToList();

                if (data.Count > 0)
                {
                    totalRecord = data[0].TotalRow;
                    filterRecord = data[0].TotalRow;
                }

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: success";
                log.ProcessName = "CallReport/GetCallReportList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                data.ForEach(p => p.strNextFollowUpDate = (p.NextFollowUpDate == null ? "" : p.NextFollowUpDate.Value.ToString("dd-MMM-yyyy")));
               

                return Json(new { draw = draw, recordsTotal = totalRecord, recordsFiltered = filterRecord, data = data });

                //return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetCallActivityDetails(string callId)
        {
            string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                CallReportActivity callActivity = new CallReportActivity();
                string message = "Sorry!!! No Data Found!!!";
                callActivity.CallId = callId;
                          
                var data = await _unitOfWork.CallReportRepository.GetCallActivityDetailsAsync(callActivity, actionType: ActionType.GetByActivityCallId.ToString());

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: success";
                log.ProcessName = "CallReport/GetCallActivityDetails";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + ex.Message;
                log.ProcessName = "CallReport/GetCallActivityDetails";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetSubordiateCallReportList()
        {
            try
            {
                int totalRecord = 0;
                int filterRecord = 0;

                var draw = Request.Form["draw"].FirstOrDefault();
                string pageSize = (Request.Form["length"].FirstOrDefault() ?? "0");
                string skip = (Request.Form["start"].FirstOrDefault() ?? "0");

                CallPlan callPlan = new CallPlan(); ;
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
               // supervisorId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                string message = "Sorry!!! No Data Found!!!";
                callPlan.EmpPin = userId;
                callPlan.Status = CallPlanStatus.APPROVED.ToString();
                callPlan.ProcessId = Convert.ToInt32(processId);
                callPlan.strTentativeDate = DateTime.Now.ToString("dd-MMM-yyyy");

                 var data = (await _unitOfWork.CallReportRepository.GetCallReportAsync(userId, processId, string.Empty
                    , pageSize: pageSize, skip: skip, actionType: ActionType.GetCallReportBySubordinate.ToString())).ToList();

                if (data.Count > 0)
                {
                    totalRecord = data[0].TotalRow;
                    filterRecord = data[0].TotalRow;
                }

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: success";
                log.ProcessName = "CallReport/GetSubordiateCallReportList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                data.ForEach(p => p.strNextFollowUpDate = (p.NextFollowUpDate == null ? "" : p.NextFollowUpDate.Value.ToString("dd-MMM-yyyy")));


                return Json(new { draw = draw, recordsTotal = totalRecord, recordsFiltered = filterRecord, data = data });

                //return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetSubordiateReportByTeam(string teamId)
        
        {
            try
            {
                int totalRecord = 0;
                int filterRecord = 0;

                var draw = Request.Form["draw"].FirstOrDefault();
                string pageSize = (Request.Form["length"].FirstOrDefault() ?? "0");
                string skip = (Request.Form["start"].FirstOrDefault() ?? "0");

                CallPlan callPlan = new CallPlan(); ;
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                // supervisorId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                string message = "Sorry!!! No Data Found!!!";
                callPlan.EmpPin = userId;
                callPlan.Status = CallPlanStatus.APPROVED.ToString();
                callPlan.ProcessId = Convert.ToInt32(processId);
                callPlan.strTentativeDate = DateTime.Now.ToString("dd-MMM-yyyy");

                var data = (await _unitOfWork.CallReportRepository.GetCallReportAsyncByTeam(userId,teamId, processId, string.Empty
                   , pageSize: pageSize, skip: skip, actionType: ActionType.GetCallReportBySubordinate.ToString())).ToList();

                if (data.Count > 0)
                {
                    totalRecord = data[0].TotalRow;
                    filterRecord = data[0].TotalRow;
                }

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: success";
                log.ProcessName = "CallReport/GetSubordiateCallReportList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                data.ForEach(p => p.strNextFollowUpDate = (p.NextFollowUpDate == null ? "" : p.NextFollowUpDate.Value.ToString("dd-MMM-yyyy")));


                return Json(new { draw = draw, recordsTotal = totalRecord, recordsFiltered = filterRecord, data = data });

                //return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult CallSchedule()
        {
            string supervisorId = string.Empty;

            string role = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;

            if (role == AppRole.ADMIN || role==AppRole.PROCESS_ADMIN || role==AppRole.LINE_MANAGER)
            {
                supervisorId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            }
            ViewBag.Supervisorid = supervisorId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAcceptedCallPlanList()
        {
            try
            {
                
                CallPlan callPlan = new CallPlan(); ;
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                string message = "Sorry!!! No Data Found!!!";
                callPlan.EmpPin = userId;
                callPlan.Status = CallPlanStatus.APPROVED.ToString();
                callPlan.ProcessId = Convert.ToInt32(processId);
                callPlan.strTentativeDate = DateTime.Now.ToString("dd-MMM-yyyy");

                var data = await _unitOfWork.CallPlanRepository.GetListAsync(callPlan, actionType: ActionType.GetByTentativeDate.ToString());

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: success" ;
                log.ProcessName = "CallReport/GetAcceptedCallPlanList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSubordinateScheduleList()
        {
            try
            {

                CallPlan callPlan = new CallPlan(); ;
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                string message = "Sorry!!! No Data Found!!!";
                callPlan.EmpPin = userId;
                callPlan.Status = CallPlanStatus.APPROVED.ToString();
                callPlan.ProcessId = Convert.ToInt32(processId);
                callPlan.strTentativeDate = DateTime.Now.ToString("dd-MMM-yyyy");
               
                var data = await _unitOfWork.CallPlanRepository.GetListAsync(callPlan, actionType: ActionType.GetBySubordinateId.ToString());

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: success";
                log.ProcessName = "CallReport/GetSubordinateScheduleList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSubordinateByTeamId(string teamId)
        {
            try
            {

                CallPlan callPlan = new CallPlan(); ;
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                string message = "Sorry!!! No Data Found!!!";
                callPlan.EmpPin = userId;
                callPlan.Status = CallPlanStatus.APPROVED.ToString();
                callPlan.ProcessId = Convert.ToInt32(processId);
                callPlan.strTentativeDate = DateTime.Now.ToString("dd-MMM-yyyy");

                var data = await _unitOfWork.CallPlanRepository.GetListByTeamIdAsync(callPlan, teamId, actionType: ActionType.GetBySubordinateId.ToString());

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: success";
                log.ProcessName = "CallReport/GetSubordinateScheduleList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> DismissCallPlan(CallPlan callPlan)
        {
            string userName = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                string message = "";
                bool saved = false;

                await _unitOfWork.CallPlanRepository.CallPlanStatusChange(CallPlanStatus.DISMISSED.ToString(),callPlan.Id.ToString(), userName, callPlan.Remarks, ActionType.Dismiss.ToString());
                message = "Call Plan dismissed successfully!!!!!";
                saved = true;

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + message;
                log.ProcessName = "CallPlan/DismissCallPlan";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);


                return Json(new { status = saved, message = message });
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


        [HttpGet]
        public async Task<IActionResult> GetCallScheduleView()
        {
            try
            {

                CallPlan callPlan = new CallPlan(); ;
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                string message = "Sorry!!! No Data Found!!!";
                callPlan.EmpPin = userId;
                callPlan.Status = CallPlanStatus.APPROVED.ToString();
                callPlan.ProcessId = Convert.ToInt32(processId);
                callPlan.strTentativeDate = DateTime.Now.ToString("dd-MMM-yyyy");


                

                var CallPlans = _unitOfWork.CallPlanRepository.GetListAsync(callPlan, actionType: ActionType.GetByTentativeDate.ToString());
                var FollowupCalls = _unitOfWork.CallReportRepository.GetFollowupCallScheduleAsync(userId, processId);

                CallScheduleViewModel callScheduleViewModel = new CallScheduleViewModel();
                callScheduleViewModel.CallPlans = CallPlans.Result.ToList();
                callScheduleViewModel.FollowupCalls = FollowupCalls.Result.ToList();

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: success";
                log.ProcessName = "CallReport/GetAcceptedCallPlanList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                return PartialView("_CallSchedule", callScheduleViewModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetPlanVsCallData()
        {
            string userName = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                int year = DateTime.Now.Year;
                DateTime firstDay = new DateTime(year, 1, 1);
                DateTime lastDay = new DateTime(year, 12, 31);
                var data = (await _unitOfWork.CallReportRepository.GetChartData(firstDay.ToString(), lastDay.ToString(), ActionType.GetChartDasboard.ToString()));

                string message = "Data Found";

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + message;
                log.ProcessName = "CallPlan/GetPlanVsCallData";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);
                bool found = true;

                return Json(new { data = data, status = found, message = message }); ;
            }
            catch (Exception ex)
            {
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + ex.Message;
                log.ProcessName = "CallPlan/GetPlanVsCallData";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                throw;
            }
        }

    }
}
