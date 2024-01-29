using CallBookSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace XCRV.Web.Views.Shared.Components.TopMenuWidget
{
    public class TopMenuWidgetViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public TopMenuWidgetViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("_TopNavigation");
        }
    }
}
