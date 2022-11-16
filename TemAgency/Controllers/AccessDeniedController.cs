using System.Web.Mvc;
namespace TemAgency.Controllers
{
    public class AccessDeniedController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}