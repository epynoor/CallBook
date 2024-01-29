using Microsoft.AspNetCore.Mvc;

namespace CallBookSystem.Web.Helpers
{
    public static class NavigationIndicatorHelper
    {
        public static string MakeActiveClass(this IUrlHelper urlHelper, string url)
        {
            try
            {
                string[] urlStr = url.Split("/");

                string controller = string.Empty;
                string action = string.Empty;
                if (urlStr.Length > 2)
                {
                    controller = urlStr[1];
                    action = urlStr[2];
                }
                else
                {
                    controller = urlStr[0];
                }


                string result = "active";
                string controllerName = urlHelper.ActionContext.RouteData.Values["controller"].ToString();
                string methodName = urlHelper.ActionContext.RouteData.Values["action"].ToString();
                if (string.IsNullOrEmpty(controllerName)) return null;
                if (controllerName.Equals(controller, StringComparison.OrdinalIgnoreCase))
                {
                    if (methodName.Equals(action, StringComparison.OrdinalIgnoreCase))
                    {
                        return result;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
