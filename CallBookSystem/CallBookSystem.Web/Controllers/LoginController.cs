using CallBookSystem.Application.Interfaces;
using CallBookSystem.Domain.Entities;
using CallBookSystem.Domain.Enums;
using CallBookSystem.Web.Helpers;
using CallBookSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CallBookSystem.Web.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private SysActivityLog log;


        public LoginController(ILogger<LoginController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            log = new SysActivityLog();
        }

        [AllowAnonymous]
        public IActionResult Index(LoginViewModel loginViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(loginViewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> SignIn(LoginViewModel loginViewModel)
        {
            if(!ModelState.IsValid)
            {
                return View("Index",loginViewModel);
            }

            if(loginViewModel == null)
            {
                loginViewModel = new LoginViewModel();
                return View("Index", loginViewModel);
            }
            else if(string.IsNullOrEmpty(loginViewModel.UserId))
            {
                return View("Index", loginViewModel);
            }
            
            HrUser user = await _unitOfWork.HrUserRepository.GetUserInfo(loginViewModel.UserId);
            
            string ResponseMessage = string.Empty;  


            if (user != null)            
            {
                
                if (!Convert.ToBoolean(user.IsActive=="1"?true:false))
                {                    
                    ResponseMessage = "Your account is not active. Contact with IT-HELPDESK.";
                    ViewBag.ResponseMessage = ResponseMessage;
                    TempData["ErrorMessage"] = ResponseMessage;                    

                    log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage:" + ResponseMessage;
                    log.ProcessName = "Login";
                    log.SurveyId = "CallBook";
                    log.EmpId = loginViewModel.UserId;
                    await _unitOfWork.LogRepository.ActivityLogEntry(log);
                    
                    return View("Index", loginViewModel); 
                }
               
                string encriptPass = Cipher.EncryptSHA256Str(loginViewModel.Password);

                user = await _unitOfWork.HrUserRepository.GetUserInfoByUserIdAndPassword(loginViewModel.UserId, encriptPass);

                if(user != null)
                {
                    _logger.LogInformation($"User found. User - {user.EmpId}");

                    if(user.IsAdmin & user.Role != AppRole.ADMIN)
                    {
                        user.Role = AppRole.PROCESS_ADMIN;
                    }
                    else if(user.IsLineManager & user.Role != AppRole.ADMIN)
                    {
                        user.Role = AppRole.LINE_MANAGER;
                    }

                    var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(25),
                        IsPersistent = true,
                        IssuedUtc = DateTimeOffset.UtcNow,
                        RedirectUri = "~/Login/Index"
                    };

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.EmpId??""),
                        new Claim(ClaimTypes.Name, user.EmpName??""),
                        new Claim(ClaimTypes.GivenName, user.EmpName??""),
                        new Claim("Department",  user.Department??""),
                        new Claim("Designation", user.Designation??""),
                        new Claim("Email", user.EmailId??""),
                        new Claim("Mobile", user.MobileNo??""),
                        new Claim(ClaimTypes.Role, user.Role??""),
                        new Claim("SupervisorId", user.SuperVisorId??""),
                        new Claim("SupervisorName", user.SuperVisorName??""),
                        new Claim("ProcessId", user.ProcessId??""),
                        new Claim("UnitId", user.UnitId??""),
                        new Claim("UnitName", user.UnitName??""),
                        new Claim("LocationId", user.LocationId??""),
                        new Claim("Location", user.Location??""),

                    };
                    var claimsIdentity = new ClaimsIdentity(claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

                    HttpClient httpClient = new HttpClient();
                        

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    ResponseMessage = $"Login Successfully. User - {user.EmpId}";
                    log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage:" + ResponseMessage;
                    log.ProcessName = "Login";
                    log.SurveyId = "CallBook";
                    log.EmpId = loginViewModel.UserId;
                    await _unitOfWork.LogRepository.ActivityLogEntry(log);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ResponseMessage = $"Wrong Password.";
                    ViewBag.ResponseMessage = ResponseMessage;
                    TempData["ErrorMessage"] = ResponseMessage;

                    log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage:" + ResponseMessage+"-"+ loginViewModel.UserId;
                    log.ProcessName = "Login";
                    log.SurveyId = "CallBook";
                    log.EmpId = loginViewModel.UserId;
                    await _unitOfWork.LogRepository.ActivityLogEntry(log);

                    return View("Index", loginViewModel);
                }
                
            }
            else
            {
                ResponseMessage = $"User Not Found.";
                ViewBag.ResponseMessage = ResponseMessage;
                TempData["ErrorMessage"] = ResponseMessage;

                log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage:" + ResponseMessage + "-" + loginViewModel.UserId;
                log.ProcessName = "Login";
                log.SurveyId = "CallBook";
                log.EmpId = loginViewModel.UserId;
                await _unitOfWork.LogRepository.ActivityLogEntry(log);

                return View("Index", loginViewModel);
            }

        }




        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            string userId = String.Empty;
            if (User.Identity.IsAuthenticated)
            {
                userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
            }            

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            log.ActivityLog = "Response.StatusCode: " + Response.StatusCode + " and ResDtls.ResponseMessage: Logout Successfully - " + userId;
            log.ProcessName = "Logout";
            log.SurveyId = "CallBook";
            log.EmpId = userId;
            await _unitOfWork.LogRepository.ActivityLogEntry(log);

            return RedirectToAction("Index");
        }

    }
}
