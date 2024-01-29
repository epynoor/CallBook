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
    public class CallTypeController : BaseController
    {
        private readonly ILogger<CallTypeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private SysActivityLog log;

        public CallTypeController(ILogger<CallTypeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            log = new SysActivityLog();
        }

        [Authorize(Roles = AppRole.ADMIN + "," + AppRole.PROCESS_ADMIN)]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllTypeList()
        {
            try
            {
                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string message = "Sorry!!! No Data Found!!!";
                var data=await _unitOfWork.CallTypeRepository.GetAllTypeAsync();

                string role = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;
                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;

                if (role != AppRole.ADMIN)
                {
                    data = data.Where(p => p.ProcessId.Equals(processId)).ToList();
                }

                return Json(new { data = data , status="success",messege= message, result=CommonAjaxResponse("success","success","200")}) ;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> SaveProcess(CallType type)
        {
            try
            {
                string message = "";
                string userName = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                bool saved = false;

                if (string.IsNullOrEmpty(type.Name))
                {
                    message = "Type Can't be empty.";
                }
                if (string.IsNullOrEmpty(type.ProcessId))
                {
                    message = "Process Can't be empty.";
                }
                else
                {
                    var etype = await _unitOfWork.CallTypeRepository.GetTypeAsync(type.Id, type.ProcessId, type.Name, ActionType.GetByName.ToString());
                    if (etype == null)
                    {
                        if (string.IsNullOrEmpty(type.Id))
                        {
                            await _unitOfWork.CallTypeRepository.SaveType(type, userName, ActionType.Insert.ToString());
                            saved = true;
                            message = "Successfully Inserted!!!";
                        }
                        else
                        {
                            await _unitOfWork.CallTypeRepository.SaveType(type, userName, ActionType.Update.ToString());
                            saved = true;
                            message = "Successfully Updated!!!";
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(type.Id) || (etype.Id != type.Id) )
                        {
                            message = "Same Lead Type Name already exists";
                        }
                        else
                        {
                            if (etype.Id == type.Id)
                            {
                                await _unitOfWork.CallTypeRepository.SaveType(type, userName, ActionType.Update.ToString());
                                saved = true;
                                message = "Successfully Updated!!!";
                            }
                        }
                    }
                }


                return Json(new { status = saved, message = message });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> CallTypeFilter(string term, int pageNumber, int pageSize)
        {
            try
            {
                term = HttpUtility.HtmlEncode(term);

                string processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                string role = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;

                

                var data = (await _unitOfWork.CallTypeRepository.GetAllTypeAsync()).ToList();
                if (role != AppRole.ADMIN)
                {
                    data = data.Where(p => p.ProcessId.Equals(processId)).ToList();
                }

                if (!string.IsNullOrEmpty(term))                
                {
                    data = (from p in data
                            where (p.Name.ToLower().Contains(term.ToLower()) || p.Id.Contains(term))
                            select p).OrderBy(c => c.Name).ToList();
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
