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
    public class UserManagementController : BaseController
    {
        private readonly ILogger<UserManagementController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private SysActivityLog log;

        public UserManagementController(ILogger<UserManagementController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            log = new SysActivityLog();
        }

        [Authorize(Roles = AppRole.ADMIN +","+AppRole.PROCESS_ADMIN+","+AppRole.LINE_MANAGER)]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoadUser(string id, string empName, int pageNumber, int loadSize)
        {
            try
            {
                string supervisorId = string.Empty;

                string role = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;

                if (role == AppRole.LINE_MANAGER)
                {
                    supervisorId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                }

                var data = (await _unitOfWork.HrUserRepository.GetAllUser(supervisorId)).ToList();
                
                if (!string.IsNullOrEmpty(empName))
                {
                    data = (from p in data
                            where p.EmpName.ToUpper().Contains(empName.ToUpper()) 
                            select p).OrderBy(c => c.EmpName).ToList();
                }
               
                

                var total = data.Count();
                data = data.Skip(loadSize * (pageNumber - 1)).Take(loadSize).ToList();
                return Json(new { data = data, status = "success", Total = total, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadUserbySupervisor(string id, string empName, int pageNumber, int loadSize)
        {
            try
            {
                string supervisorId = string.Empty;

                string role = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;

                //if (role == AppRole.LINE_MANAGER)
                {
                    supervisorId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                }

                var data = (await _unitOfWork.HrUserRepository.GetAllUserBySupervisorId(supervisorId)).ToList();

                if (!string.IsNullOrEmpty(empName))
                {
                    data = (from p in data
                            where p.EmpName.ToUpper().Contains(empName.ToUpper())
                            select p).OrderBy(c => c.EmpName).ToList();
                }



                var total = data.Count();
                data = data.Skip(loadSize * (pageNumber - 1)).Take(loadSize).ToList();
                return Json(new { data = data, status = "success", Total = total, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetUserManageList()
        {
            try
            {
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string message = "Sorry!!! No Data Found!!!";
                var data = await _unitOfWork.UserManagementRepository.GetAllUserManagementAsync();

                string role = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;

                if (role != AppRole.ADMIN)
                {
                    data = data.Where(p => p.ProcessId.Equals(processId)).ToList();
                }

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: Get User Management list ";
                log.ProcessName = "UserManagement/GetUserManageList";
                log.SurveyId = "CallBook";
                log.EmpId = userId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserManagement(UserManagement userManagement)
        {
            string logName = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                string message = "";
                bool saved = false;

                if (string.IsNullOrEmpty(userManagement.ProcessId))
                {
                    message = "User Name Can't be empty.";
                }
                if (string.IsNullOrEmpty(userManagement.UserId))
                {
                    message = "Select a user for the process.";
                }
                else
                {
                    var euserManagement = await _unitOfWork.UserManagementRepository.GetUserManagementAsync(userManagement.Id, userManagement.UserId, userManagement.ProcessId, ActionType.GetByName.ToString());
                    if (euserManagement == null)
                    {
                        if (string.IsNullOrEmpty(userManagement.Id))
                        {
                            await _unitOfWork.UserManagementRepository.SaveUserManagementAsync(userManagement, logName, ActionType.Insert.ToString());
                            saved = true;
                            message = "Successfully Inserted!!!";
                        }
                        else
                        {
                            await _unitOfWork.UserManagementRepository.SaveUserManagementAsync(userManagement, logName, ActionType.Update.ToString());
                            saved = true;
                            message = "Successfully Updated!!!";
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(userManagement.Id) || (euserManagement.Id != userManagement.Id))
                        {
                            message = "Same User Name already exists for the selected process.";
                        }
                        else
                        {
                            if (euserManagement.Id == userManagement.Id)
                            {
                                await _unitOfWork.UserManagementRepository.SaveUserManagementAsync(userManagement, logName, ActionType.Update.ToString());
                                saved = true;
                                message = "Successfully Updated!!!";
                            }
                        }
                    }
                }

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + message;
                log.ProcessName = "UserManagement/SaveCategory";
                log.SurveyId = "CallBook";
                log.EmpId = logName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);


                return Json(new { status = saved, message = message });
            }
            catch (Exception ex)
            {
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + ex.Message;
                log.ProcessName = "UserManagement/SaveCategory";
                log.SurveyId = "CallBook";
                log.EmpId = logName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                throw;
            }
        }

    }
}
