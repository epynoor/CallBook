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
    public class CategoryController : BaseController
    {
        private readonly ILogger<ProcessController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private SysActivityLog log;

        public CategoryController(ILogger<ProcessController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            log = new SysActivityLog();
        }

        [Authorize(Roles = AppRole.ADMIN + "," + AppRole.PROCESS_ADMIN)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetCategoriesList(string processId)
        {
            try
            {
                processId = HttpUtility.HtmlEncode(processId).Trim();

                string userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
                string message = "Sorry!!! No Data Found!!!";
                string role = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;
                if (role != AppRole.ADMIN)
                {
                    processId = User.Claims.Where(c => c.Type == "ProcessId").FirstOrDefault().Value;
                }

                var data = await _unitOfWork.CategoryRepository.GetAllCategoriesByProcessIdAsync(processId);

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: Get Category list ";
                log.ProcessName = "Category/GetCategoriesList";
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

        public async Task<IActionResult> SaveCategory(Category category)
        {
            string userName = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            try
            {
                string message = "";
                bool saved = false;

                if (string.IsNullOrEmpty(category.CategoryName))
                {
                    message = "Category Name Can't be empty.";
                }
                if (string.IsNullOrEmpty(category.ProcessId))
                {
                    message = "Select a Process for the category.";
                }
                else
                {
                    var ecatetory = await _unitOfWork.CategoryRepository.GetCategoryAsync(category.Id, category.CategoryName, category.ProcessId, ActionType.GetByName.ToString());
                    if (ecatetory == null)
                    {
                        if (string.IsNullOrEmpty(category.Id))
                        {
                            await _unitOfWork.CategoryRepository.SaveCategoryAsync(category, userName, ActionType.Insert.ToString());
                            saved = true;
                            message = "Successfully Inserted!!!";
                        }
                        else
                        {
                            await _unitOfWork.CategoryRepository.SaveCategoryAsync(category, userName, ActionType.Update.ToString());
                            saved = true;
                            message = "Successfully Updated!!!";
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(category.Id) || (ecatetory.Id != category.Id))
                        {
                            message = "Same Category Name already exists for the selected process.";
                        }
                        else
                        {
                            if (ecatetory.Id == category.Id)
                            {
                                await _unitOfWork.CategoryRepository.SaveCategoryAsync(category, userName, ActionType.Update.ToString());
                                saved = true;
                                message = "Successfully Updated!!!";
                            }
                        }
                    }
                }

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + message;
                log.ProcessName = "Category/SaveCategory";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);


                return Json(new { status = saved, message = message });
            }
            catch (Exception ex)
            {
                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: " + ex.Message;
                log.ProcessName = "Category/SaveCategory";
                log.SurveyId = "CallBook";
                log.EmpId = userName;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> CategoryFilterByProcess(string categoryName, int pageNumber, int pageSize)
        {
            try
            {
                categoryName = HttpUtility.HtmlEncode(categoryName);

                string ProcessId = User.Claims.Where(c => c.Type == "ProcessId").First().Value;

                var data = await _unitOfWork.CategoryRepository.GetAllCategoriesAsync();
                if (string.IsNullOrEmpty(categoryName))
                {
                    data = (from p in data
                            where p.ProcessId.Equals(ProcessId)
                            select p).OrderBy(c => c.CategoryName).ToList();
                }
                else
                {
                    data = (from p in data
                            where (p.CategoryName.ToLower().Contains(categoryName.ToLower()) || p.Id.Contains(categoryName))
                            && p.ProcessId.Equals(ProcessId)
                            select p).OrderBy(c => c.CategoryName).ToList();
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
