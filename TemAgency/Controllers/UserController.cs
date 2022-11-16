using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.WebControls;
using TemAgency.BusinessLayer;
using TemAgency.BusinessLayer.Models;

namespace TemAgency.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        DataLayer layer = new DataLayer();
        TEMAGENCYEntities tcontext = new TEMAGENCYEntities();
        ProjectUtil util = new ProjectUtil();
        private ApplicationUserManager _userManager;
        public UserController()
        {
        }
        public UserController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        public ActionResult Index()
        {
            return View(tcontext.AspNetUsers);
        }
        public ActionResult EditUser(string number)
        {
            string userRole = null;
            string userRoleId = null;
            AspNetUsers aspNetUser = layer.GetUserById(number);
            foreach (var item in aspNetUser.AspNetRoles)
            {
                userRole = item.Name;
                userRoleId = item.Id;
                break;
            }
            User user = new User()
            {
                ConfirmPassword = aspNetUser.PasswordHash,
                Email = aspNetUser.Email,
                Id = aspNetUser.Id,
                FullName = aspNetUser.FullName,
                Password = aspNetUser.PasswordHash,
                UserName = aspNetUser.UserName,
                UserRole = userRole,
                UserRoleId = userRoleId
            };
            List<SelectListItem> roleList = new List<SelectListItem>
            {
                new SelectListItem { Text = "SEÇİNİZ", Value = "" }
            };
            foreach (var item in layer.GetRoles())
            {
                roleList.Add(new SelectListItem { Text = item.Name, Value = item.Id });
            }
            ViewBag.roleList = roleList;
            ViewBag.ErrorEdit = TempData["ErrorEdit"];
            return View(user);
        }
        [HttpPost]
        public ActionResult EditUser(User user)
        {
            string error = "";
            if (layer.GetUsers().Where(x => x.Email == user.Email && x.Id != user.Id).Count() > 0)
            {
                error += "Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.<br/>";
            }
            if (String.IsNullOrEmpty(user.FullName))
            {
                error += "Adı Soyadı alanı boş geçilemez.<br/>";
            }
            if (String.IsNullOrEmpty(user.Email))
            {
                error += "E-Posta alanı boş geçilemez.<br/>";
            }
            if (String.IsNullOrEmpty(user.UserRoleId))
            {
                error += "Lütfen rol seçiniz.<br/>";
            }
            if (error.Length > 1)
            {
                error = error.Substring(0, error.Length - 5);
                TempData["ErrorEdit"] = error;
                return RedirectToAction("EditUser", "User", new { number = user.Id });
            }
            else
            {
                var role = layer.GetRoleById(user.UserRoleId);
                AddUserToRole(user.Id, role.Name);
            }
            AspNetUsers aspNetUser = layer.GetUserById(user.Id);
            aspNetUser.FullName = user.FullName;
            aspNetUser.Email = user.Email;
            layer.EditUser(aspNetUser);
            if (User.IsInRole("Yönetici"))
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public void AddUserToRole(string userId, string roleName)
        {
            foreach (var item in UserManager.GetRoles(userId).ToList())
            {
                UserManager.RemoveFromRole(userId, item);
            }
            UserManager.AddToRole(userId, roleName);
        }
        public ActionResult ChangePassword(string number)
        {
            AspNetUsers aspNetUser = layer.GetUserById(number);
            User user = new User()
            {
                Email = aspNetUser.Email,
                Id = aspNetUser.Id,
                UserName = aspNetUser.UserName,
                FullName = aspNetUser.FullName,
            };
            List<SelectListItem> roleList = new List<SelectListItem>
            {
                new SelectListItem { Text = "SEÇİNİZ", Value = "" }
            };
            foreach (var item in layer.GetRoles())
            {
                roleList.Add(new SelectListItem { Text = item.Name, Value = item.Id });
            }
            ViewBag.roleList = roleList;
            ViewBag.Error = TempData["Error"];
            return View(user);
        }
        [HttpPost]
        public ActionResult ChangePassword(User user)
        {
            if (String.IsNullOrEmpty(user.OldPassword))
            {
                string error = "Lütfen eski şifreyi giriniz.";
                TempData["Error"] = error;
                return RedirectToAction("ChangePassword", "User", new { number = user.Id });
            }
            if (String.IsNullOrEmpty(user.Password))
            {
                string error = "Şifre boş geçilemez.";
                TempData["Error"] = error;
                return RedirectToAction("ChangePassword", "User", new { number = user.Id });
            }
            if (user.Password != user.ConfirmPassword)
            {
                string error = "Şifreler eşleşmemektedir.";
                TempData["Error"] = error;
                return RedirectToAction("ChangePassword", "User", new { number = user.Id });
            }
            IdentityResult result = UserManager.ChangePassword(user.Id, user.OldPassword, user.Password);
            if (!result.Succeeded)
            {
                string identityError = "";
                foreach (var item in result.Errors)
                {
                    identityError += item;
                }
                if (identityError.Contains("Passwords must be at least 6 characters."))
                {
                    identityError = identityError.Replace("Passwords must be at least 6 characters.", "Şifre en az 6 karakter olmalıdır.<br/>");
                }
                if (identityError.Contains("Passwords must have at least one non letter or digit character."))
                {
                    identityError = identityError.Replace("Passwords must have at least one non letter or digit character.", "Şifre en az bir özel karakter içermelidir.<br/>");
                }
                if (identityError.Contains("Passwords must have at least one digit ('0'-'9')."))
                {
                    identityError = identityError.Replace("Passwords must have at least one digit ('0'-'9').", "Şifre en az bir rakam içermelidir.<br/>");
                }
                if (identityError.Contains("Passwords must have at least one uppercase ('A'-'Z')."))
                {
                    identityError = identityError.Replace("Passwords must have at least one uppercase ('A'-'Z').", "Şifre en az bir büyük harf içermelidir.<br/>");
                }
                if (identityError.Contains("Passwords must have at least one lowercase ('a'-'z')."))
                {
                    identityError = identityError.Replace("Passwords must have at least one lowercase ('a'-'z').", "Şifre en az bir küçük harf içermelidir.<br/>");
                }
                if (identityError.Contains("Incorrect password."))
                {
                    identityError = identityError.Replace("Incorrect password.", "Eski şifreyi yanlış girdiniz.<br/>");
                }
                identityError.Substring(0, identityError.Length - 5);
                TempData["Error"] = identityError;
                return RedirectToAction("ChangePassword", "User", new { number = user.Id });
            }
            if (User.IsInRole("Yönetici"))
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        public ActionResult NewUser()
        {
            User user = new User();
            List<SelectListItem> roleList = new List<SelectListItem>
            {
                new SelectListItem { Text = "SEÇİNİZ", Value = "" }
            };
            foreach (var item in layer.GetRoles())
            {
                roleList.Add(new SelectListItem { Text = item.Name, Value = item.Id });
            }
            ViewBag.roleList = roleList;
            ViewBag.ErrorAdd = TempData["ErrorAdd"];
            return View(user);
        }
        public ActionResult ConfirmEmail(string userId, string token)
        {
            UserManager.ConfirmEmail(userId, token);
            return RedirectToAction("ConfirmEmail", "Account");
        }
        [HttpPost]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult SendConfirmEmail(string userId)
        {
            string token = UserManager.GenerateEmailConfirmationToken(userId);
            util.SendConfirmationEmail(userId, token);
            return Json(userId, JsonRequestBehavior.AllowGet);
        }
    }
}