using CallBookSystem.Application.Interfaces;
using CallBookSystem.Domain.Entities;
using CallBookSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web;

namespace CallBookSystem.Web.Controllers
{
   // [Authorize(Roles = AppRole.ADMIN+","+AppRole.LINE_MANAGER)]
    public class CallPlanApproveController : BaseController
    {
        private readonly ILogger<CallPlanApproveController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private SysActivityLog log;

        public CallPlanApproveController(ILogger<CallPlanApproveController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            log = new SysActivityLog();
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetList(string status)
        {
            try
            {
                status = HttpUtility.HtmlEncode(status);
                CallPlan callPlan = new CallPlan();
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                string message = "Sorry!!! No Data Found!!!";

                callPlan.EmpPin = userId;
                callPlan.Status = status;
                callPlan.ProcessId = Convert.ToInt32(processId);
                var data = (await _unitOfWork.CallPlanRepository.GetListAsync(callPlan, actionType: ActionType.GetByStatusAndSupervisorId.ToString())).ToList();

                data.ForEach(p => p.strTentativeDate = p.TentativeDate.ToString("dd-MMM-yyyy"));
                return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetApprovedList(string status)
        {
            try
            {
                status = HttpUtility.HtmlEncode(status);
                CallPlan callPlan = new CallPlan(); ;
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string message = "Sorry!!! No Data Found!!!";
                callPlan.EmpPin = userId;
                callPlan.Status = "Approved";
                var data = await _unitOfWork.CallPlanRepository.GetListAsync(callPlan, actionType: ActionType.GetAllBatchByEmpPin.ToString());
                return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetUpdateList(string batchno)
        {
            try
            {
                CallPlan callPlan = new CallPlan();
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string message = "Sorry!!! No Data Found!!!";
                var data = (await _unitOfWork.CallPlanRepository.GetUpdateListByBatchAsync(batchno, actionType: ActionType.GetByBatchNo.ToString())).ToList();

                data.ForEach(p => p.strTentativeDate = p.TentativeDate.ToString("dd-MMM-yyyy"));
                return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApprovePlan(string id)
        {
            string userName = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                string message = "";
                string isAccepted = "1";
                bool saved = false;

                await _unitOfWork.CallPlanRepository.ApproveCallPlan(CallPlanStatus.APPROVED.ToString(),id, userName,isAccepted, ActionType.Approve.ToString());
                message = "Call Plan approved successfully!!!!!";
                saved = true;

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + message;
                log.ProcessName = "CallPlan/SaveTempAsync";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);


                return Json(new { status = saved, message = message });
            }
            catch (Exception ex)
            {
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + ex.Message;
                log.ProcessName = "CallPlan/SaveTempAsync";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                throw;
            }
        }


        [HttpPost]
        public async Task<IActionResult> PlanNotExcepted(string id)
        {
            string userName = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                string message = "";
                string isAccepted = "1";
                bool saved = false;

                await _unitOfWork.CallPlanRepository.ApproveCallPlan(CallPlanStatus.NOT_EXCEPTED.ToString(), id, userName, isAccepted, ActionType.PlanNotExcepted.ToString());
                message = "Call Plan approved successfully!!!!!";
                saved = true;

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + message;
                log.ProcessName = "CallPlan/SaveTempAsync";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);


                return Json(new { status = saved, message = message });
            }
            catch (Exception ex)
            {
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + ex.Message;
                log.ProcessName = "CallPlan/SaveTempAsync";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTodoListView(string status, int pageSize, int pageNumber)
        {
            try
            {
                status = HttpUtility.HtmlEncode(status);
                CallPlan callPlan = new CallPlan(); ;
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                string message = "Sorry!!! No Data Found!!!";

                callPlan.EmpPin = userId;
                callPlan.Status = status;
                callPlan.ProcessId = Convert.ToInt32(processId);

                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 5;

                int skip =  (pageNumber - 1) * pageSize;
                var data = (await _unitOfWork.CallPlanRepository.GetListAsync(callPlan, actionType: ActionType.GetByStatusAndSupervisorIdByPage.ToString(), pagesize: pageSize, skip: skip)).ToList();

                data.ForEach(p => p.strTentativeDate = p.TentativeDate.ToString("dd-MMM-yyyy"));
                return PartialView("_TodoList", data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
