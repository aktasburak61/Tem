using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TemAgency.BusinessLayer;
using TemAgency.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System;
using TemAgency.BusinessLayer.Models;

namespace TemAgency.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        DataLayer layer = new DataLayer();
        ProjectUtil util = new ProjectUtil();
        public AccountController()
        {
        }
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ErrorLogin = TempData["ErrorLogin"];
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            if (result == SignInStatus.Failure)
            {
                TempData["ErrorLogin"] = "Kullanıcı adı veya şifreyi yanlış girdiniz.";
                return RedirectToAction("Login", "Account", new { returnUrl });
            }
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
            }
            return View(model);
        }
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            if (result == SignInStatus.Failure)
            {
                return View(model);
            }
            else
            {
                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(model.ReturnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                }
                return View();
            }
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        [AllowAnonymous]
        public ActionResult Register(User model)
        {
            string error = "";
            if (String.IsNullOrEmpty(model.Password))
            {
                error += "Şifre boş geçilemez.<br/>";
            }
            if (String.IsNullOrEmpty(model.FullName))
            {
                error += "Adı Soyadı alanı boş geçilemez.<br/>";
            }
            if (String.IsNullOrEmpty(model.UserRoleId))
            {
                error += "Lütfen rol seçiniz.<br/>";
            }
            if (model.Password != model.ConfirmPassword)
            {
                error += "Şifreler eşleşmemektedir.<br/>";
            }
            if(error.Length > 1)
            {
                error = error.Substring(0, error.Length - 5);
                TempData["ErrorAdd"] = error;
                return RedirectToAction("NewUser", "User");
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
            };
            var result = UserManager.Create(user, model.Password);
            if (result.Succeeded)
            {
                var role = layer.GetRoleById(model.UserRoleId);
                string token = UserManager.GenerateEmailConfirmationToken(user.Id);
                util.SendConfirmationEmail(user.Id, token);
                AddUserToRole(user.Id, role.Name);
                return RedirectToAction("RegisterConfirmation", "Account");
            }
            else
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
                if (identityError.Contains("Name cannot be null or empty."))
                {
                    identityError = identityError.Replace("Name cannot be null or empty.", "Kullanıcı adı alanı boş geçilemez.<br/>");
                }
                if (identityError.Contains("Email cannot be null or empty."))
                {
                    identityError = identityError.Replace("Email cannot be null or empty.", "E-Posta alanı boş geçilemez.<br/>");
                }
                if (identityError.Contains("Email '" + model.Email + "' is already taken."))
                {
                    identityError = identityError.Replace("Email '" + model.Email + "' is already taken.", "'" + model.Email + "' email adresi daha önce alınmıştır.<br/>");
                }
                if (identityError.Contains("Name " + model.UserName + " is already taken."))
                {
                    identityError = identityError.Replace("Name " + model.UserName + " is already taken.", "'" + model.UserName + "' kullanıcı adı daha önce alınmıştır.<br/>");
                }
                if (identityError.Contains("Email '" + model.Email + "' is invalid."))
                {
                    identityError = identityError.Replace("Email '" + model.Email + "' is invalid.", "'" + model.Email + "' geçerli bir e-posta adresi değildir.<br/>");
                }
                identityError = identityError.Substring(0, identityError.Length - 5);
                TempData["ErrorAdd"] = identityError;
                return RedirectToAction("NewUser", "User");
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
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewBag.ErrorReset = TempData["ErrorReset"];
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var aspUser = layer.GetUserByEmail(model.Email);
            if (aspUser == null)
            {
                TempData["ErrorReset"] = "Girilen maile ait kullanıcı bulunamadı.";
                return RedirectToAction("ForgotPassword", "Account");
            }
            var user = await UserManager.FindByNameAsync(aspUser.UserName);
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, protocol: Request.Url.Scheme);
            util.SendResetPasswordEmail(user.Id, callbackUrl);
            return RedirectToAction("ForgotPasswordConfirmation", "Account");
        }
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            ViewBag.ErrorPwReset = TempData["ErrorPwReset"];
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var aspUser = layer.GetUserByEmail(model.Email);
            if (aspUser == null)
            {
                TempData["ErrorPwReset"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("ResetPassword", "Account", new { code = model.Code });
            }
            if (String.IsNullOrEmpty(model.Password))
            {
                TempData["ErrorPwReset"] = "Şifre boş geçilemez.";
                return RedirectToAction("ResetPassword", "Account", new { code = model.Code });
            }
            if (model.Password != model.ConfirmPassword)
            {
                TempData["ErrorPwReset"] = "Şifreler eşleşmemektedir.";
                return RedirectToAction("ResetPassword", "Account", new { code = model.Code });
            }
            var user = await UserManager.FindByNameAsync(aspUser.UserName);
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            else
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
                if (identityError.Contains("Email cannot be null or empty."))
                {
                    identityError = identityError.Replace("Email cannot be null or empty.", "E-Posta alanı boş geçilemez.<br/>");
                }
                if (identityError.Contains("Email '" + model.Email + "' is invalid."))
                {
                    identityError = identityError.Replace("Email '" + model.Email + "' is invalid.", "'" + model.Email + "' geçerli bir e-posta adresi değildir.<br/>");
                }
                if (identityError.Contains("Invalid token."))
                {
                    identityError = identityError.Replace("Invalid token.", "Token süresi geçti, daha önce kullanıldı veya token yollanan kullanıcı dışında işlem yapmaya çalışılıyor.<br/>");
                }
                identityError = identityError.Substring(0, identityError.Length - 5);
                TempData["ErrorPwReset"] = identityError;
                return RedirectToAction("ResetPassword", "Account", new { code = model.Code });
            }
        }
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }
            var info = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return View("ExternalLoginFailure");
            }
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await UserManager.AddLoginAsync(user.Id, info.Login);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToLocal(returnUrl);
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }
            base.Dispose(disposing);
        }
        #region Helpers
        private const string XsrfKey = "XsrfId";
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }
            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }
            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }
            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        public ActionResult RegisterConfirmation()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult ConfirmEmail()
        {
            return View();
        }
        #endregion
    }
}