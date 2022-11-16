using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;
using System.Web.Services;
using TemAgency.BusinessLayer;
using TemAgency.BusinessLayer.Models;
using static TemAgency.BusinessLayer.Models.jP_Project;

namespace TemAgency.Controllers
{
    public class ProjectsController : Controller
    {
        JPLATFORMAEntities jcontext = new JPLATFORMAEntities();
        TEMAGENCYEntities tcontext = new TEMAGENCYEntities();
        DataLayer layer = new DataLayer();
        public ActionResult AllProjects()
        {
            List<Ntl_Projects> ntl_projects = tcontext.Ntl_Projects.ToList();
            List<Projects> projects = new List<Projects>();
            foreach (var item in ntl_projects)
            {
                var company = layer.GetCompanyById(item.Company ?? 0);
                var brand = layer.GetBrandById(item.Brand ?? 0);
                var customerName = layer.GetCustomerNameById(item.CustomerName ?? 0);
                var sector = layer.GetSectorById(item.Sector ?? 0);
                var subSector = layer.GetSubSectorById(item.SubSector ?? 0);
                var agency = layer.GetCustomerNameById(item.AgencyId ?? 0);
                var statusStr = layer.GetStatusStr(item.Status ?? 0);
                var agencyProject = layer.GetCustomerNameById(item.AgencyId ?? 0);
                var user = layer.GetUserById(item.UserId);
                Projects project = new Projects()
                {
                    Agency = item.Agency,
                    Company = company.DESCRIPTION,
                    CompanyId = company.LOGICALREF,
                    Brand = brand.Name,
                    BrandId = brand.Id,
                    CustomerName = customerName.DESCRIPTION,
                    CustomerNameId = customerName.LOGICALREF,
                    Id = item.Id,
                    Origin = item.Origin,
                    ProjectCode = item.ProjectCode,
                    ProjectDate = item.ProjectDate ?? DateTime.Now,
                    ProjectName = item.ProjectName,
                    Sector = sector.Description,
                    SectorId = sector.Id,
                    SubSector = subSector.Description,
                    SubSectorId = subSector.Id,
                    Status = item.Status ?? 0,
                    StatusStr = statusStr,
                    AgencyId = agencyProject != null ? agencyProject.LOGICALREF : 0,
                    AgencyName = agencyProject != null ? agencyProject.DESCRIPTION : "",
                    UserId = item.UserId,
                    User = user.FullName,
                };
                projects.Add(project);
            }
            return View(projects.AsQueryable().OrderByDescending(x => x.Id));
        }
        [AccessDeniedAuthorize(Roles = "Yönetici")]
        public ActionResult NewProject()
        {
            var user = layer.GetUserByUserName(User.Identity.Name);
            Projects project = new Projects
            {
                ProjectCode = layer.GetNextProjectCode(),
                ProjectDate = DateTime.Today,
                UserId = user.Id,
                User = user.FullName
            };
            List<Firms> firms = new List<Firms>();
            foreach (var item in jcontext.S_ORGUNITS.Where(x => x.PARENTREF == 0).ToList())
            {
                Firms firm = new Firms()
                {
                    LOGICALREF = item.LOGICALREF,
                    DESCRIPTION = item.DESCRIPTION,
                };
                firms.Add(firm);
            }
            ViewBag.Firms = firms;

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

            List<SubSectors> subSectors = new List<SubSectors>();
            foreach (var item in tcontext.Ntl_SubSector.ToList())
            {
                SubSectors subSector = new SubSectors()
                {
                    Id = item.Id,
                    SectorId = item.SectorId ?? 0,
                    Description = item.Description,
                };
                subSectors.Add(subSector);
            }
            ViewBag.SubSectors = subSectors;

            List<Brands> brands = new List<Brands>();
            foreach (var item in tcontext.Ntl_Brand.ToList())
            {
                Brands brand = new Brands()
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                brands.Add(brand);
            }
            ViewBag.Brands = brands;

            List<Arps> arps = new List<Arps>();
            foreach (var item in jcontext.U_001_ARPS)
            {
                Arps arp = new Arps()
                {
                    LOGICALREF = item.LOGICALREF,
                    DESCRIPTION = item.DESCRIPTION,
                };
                arps.Add(arp);
            }
            ViewBag.Customers = arps;

            List<SelectListItem> origins = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "YURTİÇİ",
                    Value = "1",
                },
                new SelectListItem
                {
                    Text = "YURTDIŞI",
                    Value = "2",
                }
            };
            ViewBag.Origins = origins;
            List<SelectListItem> agency = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Ajans",
                    Value = "1",
                },
                new SelectListItem
                {
                    Text = "Direk Satış",
                    Value = "2",
                }
            };
            ViewBag.Agency = agency;
            return View(project);
        }
        [HttpPost]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult SaveProject(Projects project)
        {
            List<Ntl_ProjectLines> ntl_projectLines = new List<Ntl_ProjectLines>();
            Ntl_Projects ntl_Projects = new Ntl_Projects()
            {
                Id = 0,
                Agency = project.Agency,
                Company = project.CompanyId,
                Brand = project.BrandId,
                CustomerName = project.CustomerNameId,
                AgencyId = project.AgencyId,
                Origin = project.Origin,
                ProjectCode = project.ProjectCode,
                ProjectDate = project.ProjectDate,
                ProjectName = project.ProjectName,
                Sector = project.SectorId,
                SubSector = project.SubSectorId,
                Status = 1,
                UserId = project.UserId,
            };
            foreach (var line in project.ProjectLines)
            {
                List<Ntl_ProjectLineSteps> ntl_projectLineSteps = new List<Ntl_ProjectLineSteps>();
                Ntl_ProjectLines ntl_projectLine = new Ntl_ProjectLines()
                {
                    Id = 0,
                    Agency = line.IsAgency ? line.AgencyId : 0,
                    BuyPrice = line.BuyPrice,
                    Influencer = line.InfluencerId,
                    DocumentType = line.DocumentTypeId,
                    IsAgency = line.IsAgency,
                    Live = line.Live,
                    NetTotal = line.NetTotal,
                    Platform = line.PlatformId,
                    ProjectId = 0,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    VatRate = line.VatRate,
                    VatTotal = line.VatTotal,
                    Total = line.Total,
                };
                for (int i = 0; i < line.Quantity; i++)
                {
                    Ntl_ProjectLineSteps ntl_projectLineStep = new Ntl_ProjectLineSteps()
                    {
                        Explanation = "",
                        Link = "",
                        Id = 0,
                        Price = line.NetTotal / line.Quantity,
                        ProjectLineId = 0,
                        Status = 0,
                        StepDate = DateTime.Now,
                    };
                    ntl_projectLineSteps.Add(ntl_projectLineStep);
                }
                ntl_projectLine.Ntl_ProjectLineSteps = ntl_projectLineSteps;
                ntl_projectLines.Add(ntl_projectLine);
            }
            ntl_Projects.Ntl_ProjectLines = ntl_projectLines;
            layer.SaveProject(ntl_Projects);
            return Json(project, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Project(int number)
        {
            Ntl_Projects ntl_projects = layer.GetProjectById(number);
            var company = layer.GetCompanyById(ntl_projects.Company ?? 0);
            var agencyProject = layer.GetCustomerNameById(ntl_projects.AgencyId ?? 0);
            var brand = layer.GetBrandById(ntl_projects.Brand ?? 0);
            var customerName = layer.GetCustomerNameById(ntl_projects.CustomerName ?? 0);
            var sector = layer.GetSectorById(ntl_projects.Sector ?? 0);
            var subSector = layer.GetSubSectorById(ntl_projects.SubSector ?? 0);
            var statusStr = layer.GetStatusStr(ntl_projects.Status ?? 0);
            var user = layer.GetUserById(ntl_projects.UserId);
            List<ProjectStatus> projectStatuses = new List<ProjectStatus>();
            for (int i = 1; i < 15; i++)
            {
                ProjectStatus projectStatus = new ProjectStatus()
                {
                    Value = i,
                    Text = layer.GetStatusStr(i),
                };
                projectStatuses.Add(projectStatus);
            }
            ViewBag.Statuses = projectStatuses;
            Projects projects = new Projects()
            {
                Agency = ntl_projects.Agency,
                CompanyId = company.LOGICALREF,
                Company = company.DESCRIPTION,
                BrandId = brand.Id,
                Brand = brand.Name,
                CustomerName = customerName.DESCRIPTION,
                CustomerNameId = customerName.LOGICALREF,
                Id = ntl_projects.Id,
                Origin = ntl_projects.Origin,
                ProjectCode = ntl_projects.ProjectCode,
                ProjectDate = ntl_projects.ProjectDate ?? DateTime.Now,
                ProjectName = ntl_projects.ProjectName,
                SectorId = sector.Id,
                Sector = sector.Description,
                SubSectorId = subSector.Id,
                SubSector = subSector.Description,
                Status = ntl_projects.Status ?? 0,
                StatusStr = statusStr,
                AgencyId = agencyProject != null ? agencyProject.LOGICALREF : 0,
                AgencyName = agencyProject != null ? agencyProject.DESCRIPTION : "",
                User = user.FullName,
                UserId = user.Id,
            };
            List<ProjectLines> projectLines = new List<ProjectLines>();
            foreach (var item in ntl_projects.Ntl_ProjectLines)
            {
                var agency = layer.GetCustomerNameById(item.Agency ?? 0);
                var influencer = layer.GetInfluencerById(item.Influencer ?? 0);
                var platform = layer.GetPlatformById(item.Platform ?? 0);
                var documentType = layer.GetDocumentTypeById(item.DocumentType ?? 0);
                ProjectLines projectLine = new ProjectLines()
                {
                    AgencyId = agency != null ? agency.LOGICALREF : 0,
                    Agency = agency != null ? agency.DESCRIPTION : "",
                    BuyPrice = item.BuyPrice ?? 0,
                    Id = item.Id,
                    InfluencerId = influencer.LOGICALREF,
                    Influencer = influencer.DESCRIPTION,
                    DocumentTypeId = documentType.LOGICALREF,
                    DocumentType = documentType.DESCRIPTION,
                    IsAgency = item.IsAgency ?? false,
                    Live = item.Live ?? false,
                    NetTotal = item.NetTotal ?? 0,
                    PlatformId = platform.LOGICALREF,
                    Platform = platform.DESCRIPTION,
                    ProjectId = item.ProjectId ?? 0,
                    Quantity = item.Quantity ?? 0,
                    Total = item.Total ?? 0,
                    UnitPrice = item.UnitPrice ?? 0,
                    VatRate = item.VatRate ?? 0,
                    VatTotal = item.VatTotal ?? 0,
                };
                projectLines.Add(projectLine);
            }
            projects.ProjectLines = projectLines;
            List<ProjectDocs> projectDocs = new List<ProjectDocs>();
            foreach (var item in ntl_projects.Ntl_ProjectDocs)
            {
                ProjectDocs projectDoc = new ProjectDocs()
                {
                    Id = item.Id,
                    ProjectId = item.ProjectId ?? 0,
                    DocumentContentType = item.DocumentContentType,
                    DocumentIcon = item.DocumentIcon,
                    DocumentName = item.DocumentName,
                    DocumentDate = item.DocumentDate ?? DateTime.Now,
                };
                projectDocs.Add(projectDoc);
            }
            projects.ProjectDocs = projectDocs;
            return View(projects);
        }
        [HttpGet]
        public JsonResult GetSubSector(int? number)
        {
            List<SubSectors> subSectors = new List<SubSectors>();
            if (number > 0)
            {
                SubSectors first = new SubSectors()
                {
                    Id = 0,
                    SectorId = 0,
                    Description = "Alt Sektör Seçiniz"
                };
                subSectors.Add(first);
                foreach (var item in tcontext.Ntl_SubSector.Where(x => x.SectorId == number).ToList())
                {
                    SubSectors subSector = new SubSectors()
                    {
                        Id = item.Id,
                        SectorId = item.Id,
                        Description = item.Description,
                    };
                    subSectors.Add(subSector);
                }
                return Json(new { data = subSectors }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                SubSectors first = new SubSectors()
                {
                    Id = 0,
                    SectorId = 0,
                    Description = "Alt Sektör Seçiniz"
                };
                subSectors.Add(first);
                return Json(new { data = subSectors }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetPlatform()
        {
            List<Platforms> platforms = new List<Platforms>();
            Platforms first = new Platforms()
            {
                LOGICALREF = 0,
                DESCRIPTION = "Platform Seçiniz",
            };
            platforms.Add(first);
            foreach (var item in jcontext.U_001_AUXCODES)
            {
                Platforms platform = new Platforms()
                {
                    LOGICALREF = item.LOGICALREF,
                    DESCRIPTION = item.DESCRIPTION,
                };
                platforms.Add(platform);
            }
            return Json(new { data = platforms }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetInfluencer()
        {
            List<Influencers> influencers = new List<Influencers>();
            Influencers first = new Influencers()
            {
                LOGICALREF = 0,
                DESCRIPTION = "Influencer Seçiniz",
            };
            influencers.Add(first);
            foreach (var item in jcontext.U_001_ANLYDIMENSIONS.Where(x => x.BOSTATUS == 0))
            {
                Influencers influencer = new Influencers()
                {
                    LOGICALREF = item.LOGICALREF,
                    DESCRIPTION = item.DESCRIPTION,
                };
                influencers.Add(influencer);
            }
            return Json(new { data = influencers }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetDocIdByInfluencerId(int number)
        {
            var influencer = layer.GetInfluencerById(number);
            string id = Convert.ToString(influencer.DIMTYPEREF);
            return Json(new { data = id }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAgency()
        {
            List<Agencies> agencies = new List<Agencies>();
            Agencies first = new Agencies()
            {
                LOGICALREF = 0,
                DESCRIPTION = "Ajans Seçiniz",
            };
            agencies.Add(first);
            foreach (var item in jcontext.U_001_ARPS)
            {
                Agencies agency = new Agencies()
                {
                    LOGICALREF = item.LOGICALREF,
                    DESCRIPTION = item.DESCRIPTION,
                };
                agencies.Add(agency);
            }
            return Json(new { data = agencies }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetDocumentType()
        {
            List<DocumentTypes> documentTypes = new List<DocumentTypes>();
            DocumentTypes first = new DocumentTypes()
            {
                LOGICALREF = 0,
                DESCRIPTION = "Seçiniz",
            };
            documentTypes.Add(first);
            foreach (var item in jcontext.U_001_ANLYDIMTYPES)
            {
                DocumentTypes documentType = new DocumentTypes()
                {
                    LOGICALREF = item.LOGICALREF,
                    DESCRIPTION = item.DESCRIPTION,
                };
                documentTypes.Add(documentType);
            }
            return Json(new { data = documentTypes }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult SendToLogo(int ProjectId)
        {
            string res;
            string data;
            try
            {
                jP_RestService jP_Rest = new jP_RestService();
                data = GetProjectString(ProjectId);
                jP_Rest.GetToken();
                res = jP_Rest.PostProject(data);
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        private string GetProjectString(int number)
        {
            string jsonStr;
            Ntl_Projects ntl_Project = layer.GetProjectById(number);
            jP_Project project = new jP_Project()
            {
                internal_Reference = 0,
                code = ntl_Project.ProjectCode,
                name = ntl_Project.ProjectName,
            };
            ResponsibleRef responsibleRef = new ResponsibleRef()
            {
                reference = 1,
                responsible_Code = "2211161000313044",
            };
            project.responsibleRef = responsibleRef;
            OrgUnit_FirmRef orgUnit_FirmRef = new OrgUnit_FirmRef()
            {
                reference = 1,
                orgUnit_Firm_Companynr = 1,
                orgUnit_Firm_DomainId = 0,
            };
            OrgUnitReference orgUnitReference = new OrgUnitReference()
            {
                reference = 4,
                orgUnit_Code = "01.01.01.01",
                orgUnit_DomainId = 0,
                orgUnit_FirmRef = orgUnit_FirmRef,
            };
            project.orgUnitReference = orgUnitReference;
            List<Items> items = new List<Items>();
            int i = 1;
            foreach (var line in ntl_Project.Ntl_ProjectLines)
            {
                string code = i.ToString().PadLeft(2, '0');
                var platform = layer.GetPlatformById(line.Platform ?? 0);
                Items item = new Items()
                {
                    code = ntl_Project.ProjectCode + "-" + code,
                    name = platform.DESCRIPTION,
                    actQuantity = line.Quantity ?? 0,
                    startDate = ntl_Project.ProjectDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffffffKzzz"),
                    endDate = ntl_Project.ProjectDate.Value.AddMonths(line.Quantity ?? 0).ToString("yyyy-MM-ddTHH:mm:ss.fffffffKzzz"),
                };
                items.Add(item);
                i++;
            }
            Activities activities = new Activities()
            {
                items = items,
            };
            project.activities = activities;
            jsonStr = JsonConvert.SerializeObject(project);
            return jsonStr;
        }
        private string GetProjectOrderString(int number)
        {
            string jsonStr;
            Ntl_Projects ntl_project = layer.GetProjectById(number);
            jP_Order order = new jP_Order();
            ArpOfReceipt arpOfReceipt = new ArpOfReceipt()
            {
                aRPOfReceipt_Code = "DENEME3",
                reference = 1,
            };
            order.arpOfReceipt = arpOfReceipt;
            ArpRef arpRef = new ArpRef()
            {
                aRP_Code = "DENEME3",
                reference = 1,
            };
            order.arpRef = arpRef;
            order.deductionPart1 = 2;
            order.deductionPart2 = 3;
            DepartmentFirmRef departmentFirmRef = new DepartmentFirmRef()
            {
                department_Firm_Companynr = 1,
                department_Firm_DomainId = 0,
                reference = 1,
            };
            DepartmentRef departmentRef = new DepartmentRef()
            {
                department_Code = "",
                reference = 2,
                department_DomainId = 0,
                department_FirmRef = departmentFirmRef,
            };
            order.departmentRef = departmentRef;
            DivisionFirmRef divisionFirmRef = new DivisionFirmRef()
            {
                division_Firm_Companynr = 1,
                division_Firm_DomainId = 0,
                reference = 1,
            };
            DivisionRef divisionRef = new DivisionRef()
            {
                division_Code = "01",
                reference = 1,
                division_DomainId = 0,
                division_FirmRef = divisionFirmRef,
            };
            order.divisionRef = divisionRef;
            order.orderDate = ntl_project.ProjectDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffffffKzzz");
            order.slipNumber = ntl_project.ProjectCode;
            order.slipType = 2;
            SourceWHFirmRef sourceWHFirmRef = new SourceWHFirmRef()
            {
                reference = 1,
                sourceWH_Firm_Companynr = 1,
                sourceWH_Firm_DomainId = 0,
            };
            SourceWHRef sourceWHRef = new SourceWHRef()
            {
                reference = 5,
                sourceWH_Code = "01.01.02",
                sourceWH_DomainId = 0,
                sourceWH_FirmRef = sourceWHFirmRef,
            };
            order.sourceWHRef = sourceWHRef;
            List<Item> items = new List<Item>();
            Item item = new Item();
            ARPRef2 arpRef2 = new ARPRef2()
            {
                reference = 1,
                aRP_Code = "DENEME3",
            };
            item.aRPRef = arpRef2;
            DepartmentFirmRef2 departmentFirmRef2 = new DepartmentFirmRef2()
            {
                department_Firm_Companynr = 1,
                department_Firm_DomainId = 0,
                reference = 0,
            };
            DepartmentRef2 departmentRef2 = new DepartmentRef2()
            {
                department_Code = "",
                reference = 2,
                department_DomainId = 0,
                department_FirmRef = departmentFirmRef2,
            };
            item.departmentRef = departmentRef2;
            DivisionFirmRef2 divisionFirmRef2 = new DivisionFirmRef2()
            {
                division_Firm_Companynr = 1,
                division_Firm_DomainId = 0,
                reference = 1,
            };
            DivisionRef2 divisionRef2 = new DivisionRef2()
            {
                division_Code = "01",
                reference = 1,
                division_DomainId = 0,
                division_FirmRef = divisionFirmRef2,
            };
            item.divisionRef = divisionRef2;
            List<ExtraProp> extraProps = new List<ExtraProp>();
            ExtraProp order_Extra = new ExtraProp()
            {
                name = "resolver-name",
                value = "item",
            };
            extraProps.Add(order_Extra);
            MasterReference master = new MasterReference()
            {
                reference = 3,
                extraProps = extraProps,
            };
            item.master_Reference = master;
            OrderSlipRef orderSlipRef = new OrderSlipRef()
            {
                orderSlip_SlipNumber = "Burak-000001",
                reference = 2,
                orderSlip_SlipType = 1,
            };
            item.orderSlipRef = orderSlipRef;
            item.price = 10;
            item.quantity = 5;
            item.slipDate = DateTime.Now;
            item.slipOrder = 1;
            item.slipType = 1;
            SourceWHFirmRef2 sourceWHFirmRef2 = new SourceWHFirmRef2()
            {
                reference = 0,
                sourceWH_Firm_Companynr = 0,
                sourceWH_Firm_DomainId = 0,
            };
            SourceWHRef2 sourceWHRef2 = new SourceWHRef2()
            {
                reference = 5,
                sourceWH_Code = "",
                sourceWH_DomainId = 0,
                sourceWH_FirmRef = sourceWHFirmRef2,
            };
            item.sourceWHRef = sourceWHRef2;
            item.total = 10 * 5;
            item.netTotal = 10 * 5;
            UOMUomsetref uomsetref = new UOMUomsetref()
            {
                reference = 5,
                uOM_Uomsetref_Code = "05",
            };
            UOMRef uOMRef = new UOMRef()
            {
                reference = 23,
                uOM_Code = "ADET",
                uOM_Uomsetref = uomsetref,
            };
            item.uOMRef = uOMRef;
            item.vATAmount = (item.netTotal = item.price * item.quantity) * 0.18;
            item.vATBase = item.netTotal = item.price * item.quantity;
            item.vATRate = 18;
            items.Add(item);
            Transactions transactions = new Transactions
            {
                items = items
            };
            order.transactions = transactions;
            double total = 0;
            double totalVat = 0;
            foreach (var result in items)
            {
                total += result.total;
                totalVat += result.vATAmount;
            }
            order.totalDiscounted = total;
            order.totalGross = total;
            order.totalVat = totalVat;
            order.totalNet = total + totalVat;
            jsonStr = JsonConvert.SerializeObject(order);
            return jsonStr;
        }
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file, string ProjectId)
        {
            FileUploadResult result = new FileUploadResult();
            string filename;
            try
            {
                string extension = Path.GetExtension(file.FileName).ToLower();
                filename = Path.GetFileName(file.FileName);
                if (extension == ".doc" || extension == ".docx" || extension == ".pdf" || extension == ".txt")
                {
                    string fileExtension = Path.GetExtension(file.FileName);
                    string projectCode = layer.GetProjectNrById(Convert.ToInt32(ProjectId));
                    Ntl_ProjectDocs doc = new Ntl_ProjectDocs()
                    {
                        ProjectId = Convert.ToInt32(ProjectId),
                        DocumentContentType = file.ContentType,
                        DocumentName = filename,
                        DocumentDate = DateTime.Now,
                    };
                    bool exists = Directory.Exists(Path.Combine(Server.MapPath("~/ProjectDocs/" + projectCode)));
                    if (!exists)
                        Directory.CreateDirectory(Path.Combine(Server.MapPath("~/ProjectDocs/" + projectCode)));
                    var path = Path.Combine(Server.MapPath("~/ProjectDocs/" + projectCode + "/" + doc.DocumentName));
                    file.SaveAs(path);
                    Icon img = Icon.ExtractAssociatedIcon(path);
                    doc.DocumentIcon = IconToBytes(img);
                    layer.SaveProjectDoc(doc);
                    result.Result = filename + " Dosya yüklendi.";
                    result.Success = true;
                }
                else
                {
                    result.Result = filename + " Dosya formatı uygun değildir.";
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                result.Result = ex.Message;
                result.Success = false;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public byte[] IconToBytes(Icon icon)
        {
            using (var ms = new MemoryStream())
            {
                icon.Save(ms);
                return ms.ToArray();
            }
        }
        public FileResult GetFile(string projectCode, int number)
        {
            var doc = layer.GetDocumentById(number);
            var path = Path.Combine(Server.MapPath("~/ProjectDocs/" + projectCode + "/" + doc.DocumentName));
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            if (doc.DocumentContentType.Contains("image") || doc.DocumentContentType.Contains("pdf") || doc.DocumentContentType.Contains("text") || doc.DocumentContentType.Contains("word"))
            {
                return File(fileBytes, doc.DocumentContentType, doc.DocumentName);
            }
            else
            {
                return File(fileBytes, doc.DocumentContentType, doc.DocumentName);
            }
        }
        [HttpPost]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ActionResult DeleteDocument(int documentId)
        {
            bool result = false;
            var doc = layer.GetDocumentById(documentId);
            string projectCode = layer.GetProjectNrById((int)doc.ProjectId);
            var path = Path.Combine(Server.MapPath("~/ProjectDocs/" + projectCode + "/" + doc.DocumentName));
            if (System.IO.File.Exists(path) && layer.DeleteDocumentById(documentId))
            {
                System.IO.File.Delete(path);
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Operations()
        {
            List<Ntl_Projects> ntl_projects = tcontext.Ntl_Projects.ToList();
            List<Projects> projects = new List<Projects>();
            foreach (var item in ntl_projects)
            {
                var company = layer.GetCompanyById(item.Company ?? 0);
                var brand = layer.GetBrandById(item.Brand ?? 0);
                var customerName = layer.GetCustomerNameById(item.CustomerName ?? 0);
                var sector = layer.GetSectorById(item.Sector ?? 0);
                var subSector = layer.GetSubSectorById(item.SubSector ?? 0);
                var agency = layer.GetCustomerNameById(item.AgencyId ?? 0);
                var statusStr = layer.GetStatusStr(item.Status ?? 0);
                var agencyProject = layer.GetCustomerNameById(item.AgencyId ?? 0);
                var user = layer.GetUserById(item.UserId);
                Projects project = new Projects()
                {
                    Agency = item.Agency,
                    Company = company.DESCRIPTION,
                    CompanyId = company.LOGICALREF,
                    Brand = brand.Name,
                    BrandId = brand.Id,
                    CustomerName = customerName.DESCRIPTION,
                    CustomerNameId = customerName.LOGICALREF,
                    Id = item.Id,
                    Origin = item.Origin,
                    ProjectCode = item.ProjectCode,
                    ProjectDate = item.ProjectDate ?? DateTime.Now,
                    ProjectName = item.ProjectName,
                    Sector = sector.Description,
                    SectorId = sector.Id,
                    SubSector = subSector.Description,
                    SubSectorId = subSector.Id,
                    Status = item.Status ?? 0,
                    StatusStr = statusStr,
                    AgencyId = agencyProject != null ? agencyProject.LOGICALREF : 0,
                    AgencyName = agencyProject != null ? agencyProject.DESCRIPTION : "",
                    User = user.FullName,
                    UserId = user.Id,
                };
                projects.Add(project);
            }
            return View(projects.AsQueryable().OrderByDescending(x => x.Id));
        }
        public ActionResult ProjectOperations(int number)
        {
            Ntl_Projects ntl_projects = layer.GetProjectById(number);
            var company = layer.GetCompanyById(ntl_projects.Company ?? 0);
            var brand = layer.GetBrandById(ntl_projects.Brand ?? 0);
            var customerName = layer.GetCustomerNameById(ntl_projects.CustomerName ?? 0);
            var sector = layer.GetSectorById(ntl_projects.Sector ?? 0);
            var subSector = layer.GetSubSectorById(ntl_projects.SubSector ?? 0);
            var statusStr = layer.GetStatusStr(ntl_projects.Status ?? 0);
            var agencyProject = layer.GetCustomerNameById(ntl_projects.AgencyId ?? 0);
            var user = layer.GetUserById(ntl_projects.UserId);
            List<ProjectStatus> projectStatuses = new List<ProjectStatus>();
            for (int i = 1; i < 15; i++)
            {
                ProjectStatus projectStatus = new ProjectStatus()
                {
                    Value = i,
                    Text = layer.GetStatusStr(i),
                };
                projectStatuses.Add(projectStatus);
            }
            ViewBag.Statuses = projectStatuses;
            Projects projects = new Projects()
            {
                Agency = ntl_projects.Agency,
                CompanyId = company.LOGICALREF,
                Company = company.DESCRIPTION,
                BrandId = brand.Id,
                Brand = brand.Name,
                CustomerNameId = customerName.LOGICALREF,
                CustomerName = customerName.DESCRIPTION,
                Id = ntl_projects.Id,
                Origin = ntl_projects.Origin,
                ProjectCode = ntl_projects.ProjectCode,
                ProjectDate = ntl_projects.ProjectDate ?? DateTime.Now,
                ProjectName = ntl_projects.ProjectName,
                SectorId = sector.Id,
                Sector = sector.Description,
                SubSectorId = subSector.Id,
                SubSector = subSector.Description,
                Status = ntl_projects.Status ?? 0,
                StatusStr = statusStr,
                AgencyId = agencyProject != null ? agencyProject.LOGICALREF : 0,
                AgencyName = agencyProject != null ? agencyProject.DESCRIPTION : "",
                User = user.FullName,
                UserId = user.Id,
            };
            List<ProjectLines> projectLines = new List<ProjectLines>();
            List<ProjectLineSteps> projectLineSteps = new List<ProjectLineSteps>();
            foreach (var item in ntl_projects.Ntl_ProjectLines)
            {
                var agency = layer.GetCustomerNameById(item.Agency ?? 0);
                var influencer = layer.GetInfluencerById(item.Influencer ?? 0);
                var platform = layer.GetPlatformById(item.Platform ?? 0);
                var documentType = layer.GetDocumentTypeById(item.DocumentType ?? 0);
                ProjectLines projectLine = new ProjectLines()
                {
                    AgencyId = agency != null ? agency.LOGICALREF : 0,
                    Agency = agency != null ? agency.DESCRIPTION : "",
                    BuyPrice = item.BuyPrice ?? 0,
                    Id = item.Id,
                    InfluencerId = influencer.LOGICALREF,
                    Influencer = influencer.DESCRIPTION,
                    DocumentTypeId = documentType.LOGICALREF,
                    DocumentType = documentType.DESCRIPTION,
                    IsAgency = item.IsAgency ?? false,
                    Live = item.Live ?? false,
                    NetTotal = item.NetTotal ?? 0,
                    PlatformId = platform.LOGICALREF,
                    Platform = platform.DESCRIPTION,
                    ProjectId = item.ProjectId ?? 0,
                    Quantity = item.Quantity ?? 0,
                    Total = item.Total ?? 0,
                    UnitPrice = item.UnitPrice ?? 0,
                    VatRate = item.VatRate ?? 0,
                    VatTotal = item.VatTotal ?? 0,
                    LineRemaining = item.NetTotal - layer.TotalTransfered(item.Id) ?? 0,
                };
                foreach (var step in item.Ntl_ProjectLineSteps)
                {
                    var line = layer.GetProjectLineById(step.ProjectLineId ?? 0);
                    var count = layer.UncompletedSteps(step.ProjectLineId ?? 0);
                    var total = layer.TotalTransfered(step.ProjectLineId ?? 0);
                    if (step.Status == 3)
                    {
                        ProjectLineSteps projectLineStep = new ProjectLineSteps()
                        {
                            Explanation = step.Explanation,
                            Link = step.Link,
                            Id = step.Id,
                            ProjectLineId = item.Id,
                            Status = step.Status ?? 0,
                            StepDate = step.StepDate ?? DateTime.Now,
                            Price = step.Price ?? 0,
                        };
                        projectLineSteps.Add(projectLineStep);
                    }
                    else
                    {
                        ProjectLineSteps projectLineStep = new ProjectLineSteps()
                        {
                            Explanation = step.Explanation,
                            Link = step.Link,
                            Id = step.Id,
                            ProjectLineId = item.Id,
                            Status = step.Status ?? 0,
                            StepDate = step.StepDate ?? DateTime.Now,
                            Price = ((line.NetTotal - total) / count) ?? 0,
                        };
                        projectLineSteps.Add(projectLineStep);
                    }
                }
                projectLine.ProjectLineSteps = projectLineSteps;
                projectLines.Add(projectLine);
            }
            projects.ProjectLines = projectLines;
            List<ProjectDocs> projectDocs = new List<ProjectDocs>();
            foreach (var item in ntl_projects.Ntl_ProjectDocs)
            {
                ProjectDocs projectDoc = new ProjectDocs()
                {
                    Id = item.Id,
                    ProjectId = item.ProjectId ?? 0,
                    DocumentContentType = item.DocumentContentType,
                    DocumentIcon = item.DocumentIcon,
                    DocumentName = item.DocumentName,
                    DocumentDate = item.DocumentDate ?? DateTime.Now,
                };
                projectDocs.Add(projectDoc);
            }
            List<StepStatus> stepStatuses = new List<StepStatus>();
            StepStatus stepStatus1 = new StepStatus()
            {
                Key = 1,
                Value = "Bekliyor",
            };
            StepStatus stepStatus2 = new StepStatus()
            {
                Key = 2,
                Value = "Tamamlandı",
            };
            StepStatus stepStatus3 = new StepStatus()
            {
                Key = 3,
                Value = "Logoya Aktarıldı",
            };
            stepStatuses.Add(stepStatus1);
            stepStatuses.Add(stepStatus2);
            stepStatuses.Add(stepStatus3);
            ViewBag.StepStatus = stepStatuses;
            projects.ProjectDocs = projectDocs;
            return View(projects);
        }
        [HttpPost]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult SaveOperation(List<ProjectLineSteps> projectLineSteps)
        {
            if (projectLineSteps != null)
            {
                List<Ntl_ProjectLineSteps> ntl_ProjectLineSteps = new List<Ntl_ProjectLineSteps>();
                foreach (var item in projectLineSteps)
                {
                    Ntl_ProjectLineSteps ntl_ProjectLineStep = new Ntl_ProjectLineSteps()
                    {
                        Explanation = item.Explanation,
                        Link = item.Link,
                        Id = item.Id,
                        Price = item.Price,
                        ProjectLineId = item.ProjectLineId,
                        Status = item.Status,
                        StepDate = item.StepDate,
                    };
                    ntl_ProjectLineSteps.Add(ntl_ProjectLineStep);
                }
                layer.SaveProjectLineSteps(ntl_ProjectLineSteps);
            }
            return Json(projectLineSteps, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult ChangeProjectStatus(int number, int value)
        {
            var project = layer.GetProjectById(number);
            project.Status = value;
            layer.SaveProject(project);
            return Json(new { data = value }, JsonRequestBehavior.AllowGet);
        }
    }
}