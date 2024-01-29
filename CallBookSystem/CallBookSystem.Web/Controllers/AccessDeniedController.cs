using Microsoft.AspNetCore.Mvc;

namespace CallBookSystem.Web.Controllers
{
    public class AccessDeniedController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
