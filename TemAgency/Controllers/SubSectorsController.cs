using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TemAgency.BusinessLayer;
using TemAgency.BusinessLayer.Models;

namespace TemAgency.Controllers
{
    [Authorize]
    public class SubSectorsController : Controller
    {
        TEMAGENCYEntities tcontext = new TEMAGENCYEntities();
        DataLayer layer = new DataLayer();
        public ActionResult AllSubSectors()
        {
            return View(tcontext.Ntl_SubSector.OrderByDescending(x => x.Id));
        }
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        public ActionResult AddSubSector()
        {
            Ntl_SubSector subSector = new Ntl_SubSector();
            subSector.Code = layer.GetNextSubSectorCode();
            List<Sectors> sectors = new List<Sectors>();
            foreach (var item in tcontext.Ntl_Sector.ToList())
            {
                Sectors sector = new Sectors()
                {
                    Id = item.Id,
                    Description = item.Description,
                };
                sectors.Add(sector);
            }
            ViewBag.Sectors = sectors;
            return View(subSector);
        }
        public ActionResult SaveSubSector(Ntl_SubSector model)
        {
            layer.SaveSubSector(model);
            return RedirectToAction("AllSubSectors");
        }
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        public ActionResult EditSubSector(int number)
        {
            Ntl_SubSector subSector = layer.GetSubSectorById(number);
            List<Sectors> sectors = new List<Sectors>();
            foreach (var item in tcontext.Ntl_Sector.ToList())
            {
                Sectors sector = new Sectors()
                {
                    Id = item.Id,
                    Description = item.Description,
                };
                sectors.Add(sector);
            }
            ViewBag.Sectors = sectors;
            return View(subSector);
        }
        [HttpPost]
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        public ActionResult DeleteSubSector(int number)
        {
            var subSector = layer.GetSubSectorById(number);
            layer.DeleteSubSector(subSector);
            return RedirectToAction("AllSubSectors");
        }
    }
}