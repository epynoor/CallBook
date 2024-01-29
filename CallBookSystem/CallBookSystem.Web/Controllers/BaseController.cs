using CallBookSystem.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace CallBookSystem.Web.Controllers
{
    public class BaseController : Controller
    {
        public AJaxResponse CommonAjaxResponse(string messageType, string message, string responseCode)
        {
            using (var response = new AJaxResponse())
            {
                response._AjaxCode = responseCode;
                response._DisplayMessage = message;
                response._DisplayMessageType = messageType.ToString();
                return response;
            }
        }
    }
}
