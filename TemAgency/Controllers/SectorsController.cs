using System.Linq;
using System.Web.Mvc;
using TemAgency.BusinessLayer;

namespace TemAgency.Controllers
{
    [Authorize]
    public class SectorsController : Controller
    {
        TEMAGENCYEntities tcontext = new TEMAGENCYEntities();
        DataLayer layer = new DataLayer();
        public ActionResult AllSectors()
        {
            return View(tcontext.Ntl_Sector.OrderByDescending(x => x.Id));
        }
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        public ActionResult AddSector()
        {
            Ntl_Sector sector = new Ntl_Sector();
            sector.Code = layer.GetNextSectorCode();
            return View(sector);
        }
        public ActionResult SaveSector(Ntl_Sector model)
        {
            layer.SaveSector(model);
            return RedirectToAction("AllSectors");
        }
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        public ActionResult EditSector(int number)
        {
            Ntl_Sector sector = layer.GetSectorById(number);
            return View(sector);
        }
        [HttpPost]
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        public ActionResult DeleteSector(int number)
        {
            var sector = layer.GetSectorById(number);
            layer.DeleteSector(sector);
            return RedirectToAction("AllSectors");
        }
    }
}