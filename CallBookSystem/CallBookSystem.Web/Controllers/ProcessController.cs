using CallBookSystem.Application.Interfaces;
using CallBookSystem.Domain.Entities;
using CallBookSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CallBookSystem.Web.Controllers
{
    [Authorize]
    public class ProcessController : BaseController
    {
        private readonly ILogger<ProcessController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private SysActivityLog log;

        public ProcessController(ILogger<ProcessController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            log = new SysActivityLog();
        }

        [Authorize(Roles = (AppRole.ADMIN))]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProcessList()
        {
            try
            {              
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string message = "Sorry!!! No Data Found!!!";
                var data = await _unitOfWork.ProcessRepository.GetAllProcessesAsync();

                
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: Get All process list " ;
                log.ProcessName = "Process/GetAllProcessList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [Authorize(Roles = (AppRole.ADMIN))]
        [HttpPost]
        public async Task<IActionResult> SaveProcess(Process process)
        {
            string userName = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                string message = "";                
                bool saved = false;

                if (string.IsNullOrEmpty(process.ProcessName))
                {
                    message = "Process Name Can't be empty.";
                }                
                else
                {
                    var eProcess = await _unitOfWork.ProcessRepository.GetProcessAsync(process.Id, process.ProcessName, ActionType.GetByName.ToString());
                    if (eProcess == null)
                    {
                        if (string.IsNullOrEmpty(process.Id))
                        {
                            await _unitOfWork.ProcessRepository.SaveProcessAsync(process, userName, ActionType.Insert.ToString());
                            saved = true;
                            message = "Successfully Inserted!!!";
                        }
                        else
                        {
                            await _unitOfWork.ProcessRepository.SaveProcessAsync(process, userName, ActionType.Update.ToString());
                            saved = true;
                            message = "Successfully Updated!!!";
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(process.Id) ||(eProcess.Id != process.Id))
                        {
                            message = "Same Process Name already exists";
                        }
                        else
                        {
                            if(eProcess.Id == process.Id)
                            {
                                await _unitOfWork.ProcessRepository.SaveProcessAsync(process, userName, ActionType.Update.ToString());
                                saved = true;
                                message = "Successfully Updated!!!";
                            }
                        }                        
                    }
                }

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: "+message;
                log.ProcessName = "Process/Save";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);


                return Json(new { status = saved, message = message });
            }
            catch (Exception ex)
            {
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + ex.Message;
                log.ProcessName = "Process/Save";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ProcessFilter(string id, string processName, int pageNumber, int pageSize)
        {
            try
            {
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                string role = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;

                var data = await _unitOfWork.ProcessRepository.GetAllProcessesAsync();

                if (role != AppRole.ADMIN)
                {
                    data = data.Where(p => p.Id.Equals(processId)).ToList();
                }

                if (!string.IsNullOrEmpty(processName))
                {
                    data = (from p in data
                            where ( p.ProcessName.ToLower().Contains(processName.ToLower()) || p.Id.Contains(processName))                                
                            select p).OrderBy(c=>c.ProcessName).ToList();
                }
                var total = data.Count();
                data = data.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                return Json(new { data = data, status = "success", Total = total, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
