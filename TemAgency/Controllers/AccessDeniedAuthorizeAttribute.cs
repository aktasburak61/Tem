using System.Web.Mvc;
using System.Web.Routing;
namespace TemAgency.Controllers
{
    public class AccessDeniedAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            // Redirect to the login page if necessary
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/Account/Login" + "?returnUrl=" + filterContext.HttpContext.Request.Url);
                return;
            }
            // Redirect to your "access denied" view here
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Index" }, { "controller", "AccessDenied" }, { "area", "" } });
            }
        }
    }
}