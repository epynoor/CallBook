using CallBookSystem.Application.Interfaces;
using CallBookSystem.Domain.Entities;
using CallBookSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web;

namespace CallBookSystem.Web.Controllers
{
    [Authorize]
    public class CallPlanController : BaseController
    {
        private readonly ILogger<CallPlanController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private SysActivityLog log;

        public CallPlanController(ILogger<CallPlanController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            log = new SysActivityLog();
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetList(string status)
        {
            string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                status = HttpUtility.HtmlEncode(status);
                CallPlan callPlan = new CallPlan(); ;
                
                string message = "Sorry!!! No Data Found!!!";
                callPlan.EmpPin = userId;
                callPlan.Status = status;
                var data = await _unitOfWork.CallPlanRepository.GetListAsync(callPlan, actionType: ActionType.GetAllBatchByEmpPin.ToString());

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: success";
                log.ProcessName = "CallPlan/GetList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + ex.Message;
                log.ProcessName = "CallPlan/GetList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetCallPlanListByStatus(string status)
        {
            string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                status = HttpUtility.HtmlEncode(status);

                CallPlan callPlan = new CallPlan(); ;
               
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                string message = "Sorry!!! No Data Found!!!";
                callPlan.EmpPin = userId;
                callPlan.Status = status;
                callPlan.ProcessId = Convert.ToInt32(processId);
                callPlan.strTentativeDate = DateTime.Now.ToString("dd-MMM-yyyy");

                var data = (await _unitOfWork.CallPlanRepository.GetListAsync(callPlan, actionType: ActionType.GetByStatus.ToString())).ToList();

                data.ForEach(p => p.strTentativeDate = p.TentativeDate.ToString("dd-MMM-yyyy"));

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: success";
                log.ProcessName = "CallReport/GetCallPlanListByStatus";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + ex.Message;
                log.ProcessName = "CallPlan/GetCallPlanListByStatus";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetCallPlanById(string id)
        {
            string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value; 
            try
            {
                id = HttpUtility.HtmlEncode(id);

                CallPlan callPlan = new CallPlan(); ;
               
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                string message = "Sorry!!! No Data Found!!!";
                callPlan.EmpPin = userId;
                
                callPlan.ProcessId = Convert.ToInt32(processId);

                long planId = 0;
                long.TryParse(id, out planId);
                callPlan.Id = planId;

                var data = (await _unitOfWork.CallPlanRepository.GetListAsync(callPlan, actionType: ActionType.GetById.ToString())).FirstOrDefault();
                if (data != null)
                {
                    data.strTentativeDate = data.TentativeDate.ToString("dd-MMM-yyyy");
                }

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: success";
                log.ProcessName = "CallReport/GetAcceptedCallPlanList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + ex.Message;
                log.ProcessName = "CallPlan/GetCallPlanById";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCallPlanByUser()
        {
            string userName = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                string message = "";
                bool saved = false;

                await _unitOfWork.CallPlanRepository.SaveAsync( CallPlanStatus.INITIATED.ToString(), userName, ActionType.Insert.ToString());
                message = "Call Plan created successfully!!!!!";
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

        public async Task<IActionResult> getCIFName(string cif)
        {
             CallType callPlan = new CallType();
             var data = await _unitOfWork.CallPlanRepository.GetCustomerNameByCifId(cif.Trim());
             return Json(new { data = data, status = "success", result = CommonAjaxResponse("success", "success", "200") });
        }

        #region Temp Call Plan

        [HttpPost]
        public async Task<IActionResult> GetTempList()
        {
            try
            {                
                CallPlan callPlan = new CallPlan();
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string message = "Sorry!!! No Data Found!!!";
                var data = (await _unitOfWork.CallPlanRepository.GetTempListByEmpPinAsync(userId, actionType: ActionType.GetByEmpPin.ToString())).ToList();

                data.ForEach(p => p.strTentativeDate = p.TentativeDate.ToString("dd-MMM-yyyy"));
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
            string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                CallPlan callPlan = new CallPlan();
                
                string message = "Sorry!!! No Data Found!!!";
                var data = (await _unitOfWork.CallPlanRepository.GetUpdateListByBatchAsync(batchno, actionType: ActionType.GetByBatchNo.ToString())).ToList();

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: success" ;
                log.ProcessName = "CallPlan/GetUpdateList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                data.ForEach(p => p.strTentativeDate = p.TentativeDate.ToString("dd-MMM-yyyy"));
                return Json(new { data = data, status = "success", messege = message, result = CommonAjaxResponse("success", "success", "200") });
            }
            catch (Exception ex)
            {
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + ex.Message;
                log.ProcessName = "CallPlan/GetUpdateList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);


                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveTempCallPlan(CallPlan callPlan)
        {
            string userName = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                string message = "";
                bool saved = false;

                if (string.IsNullOrEmpty(callPlan.CategoryName))
                {
                    message = "Select a Category";
                }
                else if (string.IsNullOrEmpty(callPlan.CallTypeName))
                {
                    message = "Select a Call type";
                }
                else if (string.IsNullOrEmpty(callPlan.Subject))
                {
                    message = "Subject is required";
                }
                else if (string.IsNullOrEmpty(callPlan.strTentativeDate))
                {
                    message = "Tentative Date is required";
                }
                else
                {
                    callPlan.TentativeDate = Convert.ToDateTime(callPlan.strTentativeDate);
                    callPlan.EmpPin = userName;
                    callPlan.ProcessId = Convert.ToInt32(User.Claims.Where(c => c.Type == "ProcessId").First().Value);
                    if (callPlan.BatchNo == null)
                    {
                        if (callPlan.Id > 0)
                        {
                            await _unitOfWork.CallPlanRepository.SaveTempAsync(callPlan, ActionType.Update.ToString());
                        }
                        else
                        {
                            await _unitOfWork.CallPlanRepository.SaveTempAsync(callPlan, ActionType.Insert.ToString());
                        }
                        
                    }
                    else {
                        await _unitOfWork.CallPlanRepository.UpdateCallPlan(CallPlanStatus.INITIATED.ToString(),callPlan, ActionType.Update.ToString());
                    }
                    saved = true;

                    log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: Temp Added";
                    log.ProcessName = "CallPlan/SaveTempAsync";
                    log.SurveyId = "CallBook";
                    log.EmpId = userName;
                    await _unitOfWork.LogRepository.ActivityLogEntry(log);
                }

                


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
        public async Task<IActionResult> DeleteTempCallPlan(int id ,string flag)
        {
            string userName = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                string message = "";
                bool saved = false;
                CallPlan callPlan = new CallPlan();
                callPlan.Id = id;

                if (flag != null)
                {
                    await _unitOfWork.CallPlanRepository.SaveTempAsync(callPlan, ActionType.DeleteUpdateBatch.ToString());
                }
                else {
                    await _unitOfWork.CallPlanRepository.SaveTempAsync(callPlan, ActionType.Delete.ToString());
                }
               
                saved = true;

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: Success" ;
                log.ProcessName = "CallPlan/DeleteTempCallPlan";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);


                return Json(new { status = saved, message = message });
            }
            catch (Exception ex)
            {
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + ex.Message;
                log.ProcessName = "CallPlan/DeleteTempCallPlan";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                throw;
            }
        }

        #endregion
    }
}
