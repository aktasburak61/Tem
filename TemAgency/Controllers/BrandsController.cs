using System.Linq;
using System.Web.Mvc;
using TemAgency.BusinessLayer;

namespace TemAgency.Controllers
{
    [Authorize]
    public class BrandsController : Controller
    {
        TEMAGENCYEntities tcontext = new TEMAGENCYEntities();
        DataLayer layer = new DataLayer();
        public ActionResult AllBrands()
        {
            return View(tcontext.Ntl_Brand.OrderByDescending(x => x.Id));
        }
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        public ActionResult AddBrand()
        {
            return View();
        }
        public ActionResult SaveBrand(Ntl_Brand brand)
        {
            layer.SaveBrand(brand);
            return RedirectToAction("AllBrands");
        }
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        public ActionResult EditBrand(int number)
        {
            Ntl_Brand brand = layer.GetBrandById(number);
            return View(brand);
        }
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        [HttpPost]
        public ActionResult DeleteBrand(int number)
        {
            var brand = layer.GetBrandById(number);
            layer.DeleteBrand(brand);
            return RedirectToAction("AllBrands");
        }
    }
}