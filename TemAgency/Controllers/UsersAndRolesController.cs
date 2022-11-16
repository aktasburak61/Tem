using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TemAgency.Models;
using System.Web;
using System.Web.Mvc;
namespace TemAgency.Controllers
{
    public class UsersAndRolesController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        public UsersAndRolesController()
        {
        }
        public UsersAndRolesController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }
        public UsersAndRolesController(ApplicationRoleManager roleManager)
        {
            RoleManager = roleManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        public ActionResult CreateRole(string roleName)
        {
            var role = new ApplicationRole
            {
                Name = roleName
            };
            var result = RoleManager.Create(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Role");
            }
            else
            {
                TempData["ErrorAdd"] = "Rol alanı boş geçilemez.";
                return RedirectToAction("NewRole", "Role");
            }
        }
    }
}