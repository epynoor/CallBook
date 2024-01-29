using CallBookSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace XCRV.Web.Views.Shared.Components.MainMenuWidget
{
    public class MainMenuWidgetViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public MainMenuWidgetViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //IEnumerable<MenuViewModel> finalMenu = HttpContext.Session.Get<IEnumerable<MenuViewModel>>("UserMenuList");

            //if(finalMenu == null || finalMenu.Count() ==1)
            //{
            //    var claims = HttpContext.User.Claims;

            //    string USERID = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value;
            //    string PROJECTSID = claims.FirstOrDefault(p => p.Type.Equals("PROJECTSID")).Value;
            //    string USERGROUP = claims.FirstOrDefault(p => p.Type.Equals("USERGROUP")).Value;
            //    await GetUserMenuList(USERID, PROJECTSID, USERGROUP);

            //    finalMenu = HttpContext.Session.Get<IEnumerable<MenuViewModel>>("UserMenuList");

            //    if(finalMenu == null)
            //    {
            //        finalMenu = new List<MenuViewModel>();
            //    }
            //}    


            //return View("_MainNavigation", finalMenu);

            return View("_MainNavigation");
        }


        //private async Task GetUserMenuList(string USERID, string PROJECTSID, string USERGROUP)
        //{
        //    int userId = 0;
        //    int groupId = 0;
        //    int projectId = 0;

        //    var claims = HttpContext.User.Claims;

        //    int.TryParse(USERID, out userId);
        //    int.TryParse(PROJECTSID, out projectId);
        //    int.TryParse(USERGROUP, out groupId);

        //    var allMenu = await _unitOfWork.MenuRepo.GetAllAsync();


        //    Helpers.MenuMappingList menuMappingList = new Helpers.MenuMappingList();



        //    var menuPrivilege = await _unitOfWork.MenuPrevilegeRepo.GetByUserIdAndGroupIdAndProjectId(userId, groupId, projectId);

        //    IList<MenuViewModel> menuList = new List<MenuViewModel>();

        //    IList<MenuViewModel> permissionList = new List<MenuViewModel>();

        //    IList<MenuViewModel> parent = (from p in allMenu
        //                                   where p.ParentID == 0
        //                                   select new MenuViewModel()
        //                                   {
        //                                       Id = p.MenuId,
        //                                       MenuName = p.MenuText,
        //                                       MenuUrl = p.NavigateUrl,
        //                                       SubMenu = new List<MenuViewModel>()
        //                                   }).ToList();

        //    foreach (var m in parent)
        //    {
        //        var SubMenu = (from p in allMenu
        //                       where p.ParentID == m.Id
        //                       select new MenuViewModel()
        //                       {
        //                           Id = p.MenuId,
        //                           MenuName = p.MenuText,
        //                           MenuUrl = p.NavigateUrl,
        //                           SubMenu = new List<MenuViewModel>()

        //                       }).ToList();

        //        foreach (var sm in SubMenu)
        //        {
        //            var subSubMenu = (from p in allMenu
        //                              where p.ParentID == sm.Id
        //                              select new MenuViewModel()
        //                              {
        //                                  Id = p.MenuId,
        //                                  MenuName = p.MenuText,
        //                                  MenuUrl = p.NavigateUrl,
        //                                  SubMenu = new List<MenuViewModel>()

        //                              }).ToList();

        //            if (subSubMenu.Count == 0)
        //            {
        //                var d = menuPrivilege.Where(p => (!string.IsNullOrEmpty(p.MainMenuLinkPage) && p.MainMenuLinkPage.Equals(sm.MenuUrl)) ||
        //                                                 (!string.IsNullOrEmpty(p.SubMenuLinkPage) && p.SubMenuLinkPage.Equals(sm.MenuUrl)) ||
        //                                                 (!string.IsNullOrEmpty(p.SubChildMenuLinkPage) && p.SubChildMenuLinkPage.Equals(sm.MenuUrl)));
        //                if (d.Count() > 0)
        //                {
        //                    //if (System.Diagnostics.Debugger.IsAttached)
        //                    {
        //                        var map = menuMappingList.MenuMappings.FirstOrDefault(p => p.Url.ToUpper().Equals(sm.MenuUrl.ToUpper()));
        //                        if (map != null)
        //                        {
        //                            sm.MenuUrl = map.RewriteUrl;
        //                        }
        //                    }

        //                    if (!sm.MenuUrl.ToLower().Contains("aspx"))
        //                    {
        //                        permissionList.Add(sm);
        //                        m.SubMenu.Add(sm);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                foreach (var ssm in subSubMenu)
        //                {
        //                    var d = menuPrivilege.Where(p => (!string.IsNullOrEmpty(p.MainMenuLinkPage) && p.MainMenuLinkPage.Equals(ssm.MenuUrl)) ||
        //                                                 (!string.IsNullOrEmpty(p.SubMenuLinkPage) && p.SubMenuLinkPage.Equals(ssm.MenuUrl)) ||
        //                                                 (!string.IsNullOrEmpty(p.SubChildMenuLinkPage) && p.SubChildMenuLinkPage.Equals(ssm.MenuUrl)));
        //                    if (d.Count() > 0)
        //                    {
        //                        //if (System.Diagnostics.Debugger.IsAttached)
        //                        {
        //                            var map = menuMappingList.MenuMappings.FirstOrDefault(p => p.Url.ToUpper().Equals(ssm.MenuUrl.ToUpper()));
        //                            if (map != null)
        //                            {
        //                                ssm.MenuUrl = map.RewriteUrl;
        //                            }
        //                        }

        //                        if (!ssm.MenuUrl.ToLower().Contains("aspx"))
        //                        {
        //                            permissionList.Add(ssm);
        //                            sm.SubMenu.Add(ssm);
        //                        }
        //                    }
        //                }

        //                if (sm.SubMenu.Count > 0)
        //                {
        //                    //if (System.Diagnostics.Debugger.IsAttached)
        //                    {
        //                        var map = menuMappingList.MenuMappings.FirstOrDefault(p => p.Url.ToUpper().Equals(sm.MenuUrl.ToUpper()));
        //                        if (map != null)
        //                        {
        //                            sm.MenuUrl = map.RewriteUrl;
        //                        }
        //                    }
        //                    if (!sm.MenuUrl.ToLower().Contains("aspx"))
        //                    {
        //                        permissionList.Add(sm);
        //                        m.SubMenu.Add(sm);
        //                    }
        //                }
        //            }
        //        }

        //        if (m.SubMenu.Count > 0)
        //        {
        //            //if (System.Diagnostics.Debugger.IsAttached)
        //            {
        //                var map = menuMappingList.MenuMappings.FirstOrDefault(p => p.Url.ToUpper().Equals(m.MenuUrl.ToUpper()));
        //                if (map != null)
        //                {
        //                    m.MenuUrl = map.RewriteUrl;
        //                }
        //            }

        //            if (!m.MenuUrl.ToLower().Contains("aspx"))
        //            {
        //                permissionList.Add(m);
        //                menuList.Add(m);
        //            }
        //        }
        //    }

        //    //menuList.Insert(0, new MenuViewModel()
        //    //{
        //    //    MenuName = "Home",
        //    //    MenuUrl = "",
        //    //    SubMenu = new List<MenuViewModel>()
        //    //});
        //    var finalMenu = menuList;
        //    HttpContext.Session.Set("UserMenuList", finalMenu);
        //    HttpContext.Session.Set("PermissionList", permissionList);
        //}


    }
}
