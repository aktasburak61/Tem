using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TemAgency.BusinessLayer.Models;
using TemAgency.BusinessLayer;
using System.Drawing;
using System.IO;
using System.Web.Script.Services;
using System.Web.Services;

namespace TemAgency.Controllers
{
    [Authorize]
    public class ProjectsAgreementController : Controller
    {
        TEMAGENCYEntities tcontext = new TEMAGENCYEntities();
        JPLATFORMAEntities jcontext = new JPLATFORMAEntities();
        DataLayer layer = new DataLayer();
        ProjectUtil util = new ProjectUtil();
        public ActionResult AllProjectsAgreement()
        {
            List<Ntl_ProjectsAgreement> ntl_projectsAgreements = tcontext.Ntl_ProjectsAgreement.ToList();
            List<ProjectsAgreement> projectsAgreements = new List<ProjectsAgreement>();
            var docs = layer.GetNullProjectAgreementDocs();
            if (docs.Count > 0)
            {
                foreach (var item in docs)
                {
                    layer.DeleteDocumentProjectsAgreementById(item.Id);
                }
            }
            foreach (var item in ntl_projectsAgreements)
            {
                var company = layer.GetCompanyById(item.Company ?? 0);
                var user = layer.GetUserById(item.UserId);
                string status = "";
                switch (item.Status)
                {
                    case 1:
                        status = "Avukat Onayı Bekleniyor";
                        break;
                    case 2:
                        status = "Revize İstendi";
                        break;
                    case 3:
                        status = "Avukat Tarafından Reddedildi";
                        break;
                    case 4:
                        status = "Finans Onayı Bekleniyor";
                        break;
                    case 5:
                        status = "Onaylandı";
                        break;
                    case 6:
                        status = "Proje Oluşturuldu";
                        break;
                    case 7:
                        status = "Finans Tarafından Reddedildi";
                        break;
                }
                ProjectsAgreement projectsAgreement = new ProjectsAgreement()
                {
                    Id = item.Id,
                    AgreementDate = item.AgreementDate ?? DateTime.Now,
                    CustomerName = item.CustomerName,
                    Explanation = item.Explanation,
                    LExplanation = item.LExplanation,
                    Status = item.Status ?? 0,
                    StatusStr = status,
                    UserId = item.UserId,
                    User = user.FullName,
                    CompanyId = company.LOGICALREF,
                    Company = company.DESCRIPTION,
                    FExplanation = item.FExplanation,
                    StampTax = item.StampTax ?? false,
                };
                projectsAgreements.Add(projectsAgreement);
            }
            return View(projectsAgreements.AsQueryable().OrderByDescending(x => x.Id));
        }
        public ActionResult NewProjectAgreement()
        {
            var user = layer.GetUserByUserName(User.Identity.Name);
            ProjectAgreementMM projectAgreementMM = new ProjectAgreementMM();
            projectAgreementMM.projectsAgreement.AgreementDate = DateTime.Now;
            var docs = layer.GetNullProjectAgreementDocs();
            List<ProjectAgreementDocs> projectAgreementDocs = new List<ProjectAgreementDocs>();
            foreach (var item in docs)
            {
                ProjectAgreementDocs projectAgreementDoc = new ProjectAgreementDocs()
                {
                    DocumentContentType = item.DocumentContentType,
                    DocumentIcon = item.DocumentIcon,
                    DocumentName = item.DocumentName,
                    Id = item.Id,
                    DocumentDate = item.DocumentDate ?? DateTime.Now,
                };
                projectAgreementDocs.Add(projectAgreementDoc);
            }
            projectAgreementMM.ProjectAgreementDocs = projectAgreementDocs;
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
            projectAgreementMM.projectsAgreement.UserId = user.Id;
            projectAgreementMM.projectsAgreement.User = user.FullName;
            return View(projectAgreementMM);
        }
        public ActionResult SaveProjectAgreement(ProjectAgreementMM projectsAgreementMM)
        {
            Ntl_ProjectsAgreement ntl_ProjectsAgreement = new Ntl_ProjectsAgreement()
            {
                AgreementDate = projectsAgreementMM.projectsAgreement.AgreementDate,
                CustomerName = projectsAgreementMM.projectsAgreement.CustomerName,
                Explanation = projectsAgreementMM.projectsAgreement.Explanation,
                LExplanation = projectsAgreementMM.projectsAgreement.LExplanation,
                Id = projectsAgreementMM.projectsAgreement.Id,
                Status = 1,
                UserId = projectsAgreementMM.projectsAgreement.UserId,
                Company = projectsAgreementMM.projectsAgreement.CompanyId,
                FExplanation = projectsAgreementMM.projectsAgreement.FExplanation,
                StampTax = projectsAgreementMM.projectsAgreement.StampTax,
            };
            layer.SaveProjectAgreement(ntl_ProjectsAgreement);
            foreach (var item in tcontext.Ntl_ProjectAgreementDocs.Where(x => x.ProjectAgreementId == null).ToList())
            {
                Ntl_ProjectAgreementDocs ntl_ProjectAgreementDoc = new Ntl_ProjectAgreementDocs()
                {
                    DocumentContentType = item.DocumentContentType,
                    DocumentIcon = item.DocumentIcon,
                    DocumentName = item.DocumentName,
                    DocumentDate = item.DocumentDate,
                    Id = item.Id,
                    ProjectAgreementId = ntl_ProjectsAgreement.Id,
                };
                layer.SaveProjectAgreementDoc(ntl_ProjectAgreementDoc);
            }
            util.SendEmailForProjectLawyer(ntl_ProjectsAgreement.Id);
            return RedirectToAction("AllProjectsAgreement");
        }
        public ActionResult ProjectAgreement(int number)
        {
            Ntl_ProjectsAgreement ntl_ProjectsAgreement = layer.GetProjectAgreementById(number);
            string status = "";
            var user = layer.GetUserById(ntl_ProjectsAgreement.UserId);
            var company = layer.GetCompanyById(ntl_ProjectsAgreement.Company ?? 0);
            switch (ntl_ProjectsAgreement.Status)
            {
                case 1:
                    status = "Avukat Onayı Bekleniyor";
                    break;
                case 2:
                    status = "Revize İstendi";
                    break;
                case 3:
                    status = "Avukat Tarafından Reddedildi";
                    break;
                case 4:
                    status = "Finans Onayı Bekleniyor";
                    break;
                case 5:
                    status = "Onaylandı";
                    break;
                case 6:
                    status = "Proje Oluşturuldu";
                    break;
                case 7:
                    status = "Finans Tarafından Reddedildi";
                    break;
            }
            ProjectsAgreement projectsAgreement = new ProjectsAgreement()
            {
                Id = ntl_ProjectsAgreement.Id,
                CustomerName = ntl_ProjectsAgreement.CustomerName,
                AgreementDate = ntl_ProjectsAgreement.AgreementDate ?? DateTime.Now,
                Explanation = ntl_ProjectsAgreement.Explanation,
                LExplanation = ntl_ProjectsAgreement.LExplanation,
                Status = ntl_ProjectsAgreement.Status ?? 0,
                StatusStr = status,
                UserId = user.Id,
                User = user.FullName,
                CompanyId = company.LOGICALREF,
                Company = company.DESCRIPTION,
                FExplanation = ntl_ProjectsAgreement.FExplanation,
                StampTax = ntl_ProjectsAgreement.StampTax ?? false,
            };
            List<SelectListItem> statuses = new List<SelectListItem>();
            statuses.Add(new SelectListItem
            {
                Text = "Onaylandı",
                Value = "5",
            });
            statuses.Add(new SelectListItem
            {
                Text = "Proje Oluşturuldu",
                Value = "6",
            });
            ViewBag.Statuses = statuses;
            List<ProjectAgreementDocs> projectAgreementDocs = new List<ProjectAgreementDocs>();
            foreach (var item in ntl_ProjectsAgreement.Ntl_ProjectAgreementDocs)
            {
                ProjectAgreementDocs projectAgreementDoc = new ProjectAgreementDocs()
                {
                    Id = item.Id,
                    DocumentContentType = item.DocumentContentType,
                    DocumentIcon = item.DocumentIcon,
                    DocumentName = item.DocumentName,
                    ProjectAgreementId = item.ProjectAgreementId ?? 0,
                    DocumentDate = item.DocumentDate ?? DateTime.Now,
                };
                projectAgreementDocs.Add(projectAgreementDoc);
            }
            projectsAgreement.ProjectAgreementDocs = projectAgreementDocs;
            return View(projectsAgreement);
        }
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file, string ProjectId)
        {
            if (ProjectId == "undefined")
            {
                ProjectId = null;
            }
            FileUploadResult result = new FileUploadResult();
            string filename = "";
            try
            {
                string extension = Path.GetExtension(file.FileName).ToLower();
                filename = Path.GetFileName(file.FileName);
                if (extension == ".doc" || extension == ".docx" || extension == ".pdf" || extension == ".txt")
                {
                    string fileExtension = Path.GetExtension(file.FileName);
                    Ntl_ProjectAgreementDocs doc = new Ntl_ProjectAgreementDocs()
                    {
                        ProjectAgreementId = Convert.ToInt32(ProjectId),
                        DocumentContentType = file.ContentType,
                        DocumentName = filename,
                        DocumentDate = DateTime.Now,
                    };
                    bool exists = Directory.Exists(Path.Combine(Server.MapPath("~/Sözleşmeler/")));
                    if (!exists)
                        Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Sözleşmeler/")));
                    var path = Path.Combine(Server.MapPath("~/Sözleşmeler/" + "/" + doc.DocumentName));
                    file.SaveAs(path);
                    Icon img = Icon.ExtractAssociatedIcon(path);
                    doc.DocumentIcon = IconToBytes(img);
                    layer.SaveProjectAgreementDoc(doc);
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
        public FileResult GetFileProjectsAgreement(int number)
        {
            var doc = layer.GetDocumentProjectsAgreementById(number);
            var path = Path.Combine(Server.MapPath("~/Sözleşmeler/" + doc.DocumentName));
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
        public ActionResult DeleteDocumentProjectsAgreement(int documentId)
        {
            bool result = false;
            var doc = layer.GetDocumentProjectsAgreementById(documentId);
            var path = Path.Combine(Server.MapPath("~/Sözleşmeler/" + doc.DocumentName));
            if (System.IO.File.Exists(path) && layer.DeleteDocumentProjectsAgreementById(documentId))
            {
                System.IO.File.Delete(path);
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AccessDeniedAuthorize(Roles = "Yönetici, Avukat")]
        public ActionResult ProjectAgreementOnConfirm(int number)
        {
            Ntl_ProjectsAgreement ntl_ProjectsAgreement = layer.GetProjectAgreementById(number);
            string status = "";
            var user = layer.GetUserById(ntl_ProjectsAgreement.UserId);
            var company = layer.GetCompanyById(ntl_ProjectsAgreement.Company ?? 0);
            switch (ntl_ProjectsAgreement.Status)
            {
                case 1:
                    status = "Avukat Onayı Bekleniyor";
                    break;
                case 2:
                    status = "Revize İstendi";
                    break;
                case 3:
                    status = "Avukat Tarafından Reddedildi";
                    break;
                case 4:
                    status = "Finans Onayı Bekleniyor";
                    break;
                case 5:
                    status = "Onaylandı";
                    break;
                case 6:
                    status = "Proje Oluşturuldu";
                    break;
                case 7:
                    status = "Finans Tarafından Reddedildi";
                    break;
            }
            ProjectsAgreement projectsAgreement = new ProjectsAgreement()
            {
                Id = ntl_ProjectsAgreement.Id,
                CustomerName = ntl_ProjectsAgreement.CustomerName,
                AgreementDate = ntl_ProjectsAgreement.AgreementDate ?? DateTime.Now,
                Explanation = ntl_ProjectsAgreement.Explanation,
                LExplanation = ntl_ProjectsAgreement.LExplanation,
                Status = ntl_ProjectsAgreement.Status ?? 0,
                StatusStr = status,
                UserId = user.Id,
                User = user.FullName,
                CompanyId = company.LOGICALREF,
                Company = company.DESCRIPTION,
                FExplanation = ntl_ProjectsAgreement.FExplanation,
                StampTax = ntl_ProjectsAgreement.StampTax ?? false,
            };
            List<ProjectAgreementDocs> projectAgreementDocs = new List<ProjectAgreementDocs>();
            foreach (var item in ntl_ProjectsAgreement.Ntl_ProjectAgreementDocs)
            {
                ProjectAgreementDocs projectAgreementDoc = new ProjectAgreementDocs()
                {
                    Id = item.Id,
                    DocumentContentType = item.DocumentContentType,
                    DocumentIcon = item.DocumentIcon,
                    DocumentName = item.DocumentName,
                    ProjectAgreementId = item.ProjectAgreementId ?? 0,
                    DocumentDate = item.DocumentDate ?? DateTime.Now,
                };
                projectAgreementDocs.Add(projectAgreementDoc);
            }
            projectsAgreement.ProjectAgreementDocs = projectAgreementDocs;
            return View(projectsAgreement);
        }
        [AccessDeniedAuthorize(Roles = "Yönetici, Finans")]
        public ActionResult ProjectAgreementOnConfirmFinance(int number)
        {
            Ntl_ProjectsAgreement ntl_ProjectsAgreement = layer.GetProjectAgreementById(number);
            string status = "";
            var user = layer.GetUserById(ntl_ProjectsAgreement.UserId);
            var company = layer.GetCompanyById(ntl_ProjectsAgreement.Company ?? 0);
            switch (ntl_ProjectsAgreement.Status)
            {
                case 1:
                    status = "Avukat Onayı Bekleniyor";
                    break;
                case 2:
                    status = "Revize İstendi";
                    break;
                case 3:
                    status = "Avukat Tarafından Reddedildi";
                    break;
                case 4:
                    status = "Finans Onayı Bekleniyor";
                    break;
                case 5:
                    status = "Onaylandı";
                    break;
                case 6:
                    status = "Proje Oluşturuldu";
                    break;
                case 7:
                    status = "Finans Tarafından Reddedildi";
                    break;
            }
            ProjectsAgreement projectsAgreement = new ProjectsAgreement()
            {
                Id = ntl_ProjectsAgreement.Id,
                CustomerName = ntl_ProjectsAgreement.CustomerName,
                AgreementDate = ntl_ProjectsAgreement.AgreementDate ?? DateTime.Now,
                Explanation = ntl_ProjectsAgreement.Explanation,
                LExplanation = ntl_ProjectsAgreement.LExplanation,
                Status = ntl_ProjectsAgreement.Status ?? 0,
                StatusStr = status,
                UserId = user.Id,
                User = user.FullName,
                CompanyId = company.LOGICALREF,
                Company = company.DESCRIPTION,
                FExplanation = ntl_ProjectsAgreement.FExplanation,
                StampTax = ntl_ProjectsAgreement.StampTax ?? false,
            };
            List<SelectListItem> taxes = new List<SelectListItem>();
            taxes.Add(new SelectListItem
            {
                Text = "Evet",
                Value = "1",
            });
            taxes.Add(new SelectListItem
            {
                Text = "Hayır",
                Value = "0",
            });
            ViewBag.Taxes = taxes;
            List<ProjectAgreementDocs> projectAgreementDocs = new List<ProjectAgreementDocs>();
            foreach (var item in ntl_ProjectsAgreement.Ntl_ProjectAgreementDocs)
            {
                ProjectAgreementDocs projectAgreementDoc = new ProjectAgreementDocs()
                {
                    Id = item.Id,
                    DocumentContentType = item.DocumentContentType,
                    DocumentIcon = item.DocumentIcon,
                    DocumentName = item.DocumentName,
                    ProjectAgreementId = item.ProjectAgreementId ?? 0,
                    DocumentDate = item.DocumentDate ?? DateTime.Now,
                };
                projectAgreementDocs.Add(projectAgreementDoc);
            }
            projectsAgreement.ProjectAgreementDocs = projectAgreementDocs;
            return View(projectsAgreement);
        }
        [HttpPost]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ActionResult CancelProject(int number, string lExp)
        {
            var project = layer.GetProjectAgreementById(number);
            project.Status = 3;
            project.LExplanation = lExp;
            layer.SaveProjectAgreement(project);
            util.SendEmailForProjectRejectedLawyer(number);
            return Json(Url.Action("AllProjectsAgreement", "ProjectsAgreement"));
        }
        [HttpPost]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ActionResult EditProject(int number, string lExp)
        {
            var project = layer.GetProjectAgreementById(number);
            project.Status = 2;
            project.LExplanation = lExp;
            layer.SaveProjectAgreement(project);
            util.SendEmailForProjectEditedLawyer(number);
            return Json(Url.Action("AllProjectsAgreement", "ProjectsAgreement"));
        }
        [HttpPost]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ActionResult SendEditProject(int number, string Explanation)
        {
            var project = layer.GetProjectAgreementById(number);
            project.Status = 1;
            project.Explanation = Explanation;
            layer.SaveProjectAgreement(project);
            util.SendEmailForProjectLawyer(number);
            return Json(Url.Action("AllProjectsAgreement", "ProjectsAgreement"));
        }
        [HttpPost]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ActionResult AcceptProject(int number, string lExp)
        {
            var project = layer.GetProjectAgreementById(number);
            project.Status = 4;
            project.LExplanation = lExp;
            layer.SaveProjectAgreement(project);
            util.SendEmailForProjectAcceptedLawyer(number);
            util.SendEmailForProjectFinance(number);
            return Json(Url.Action("AllProjectsAgreement", "ProjectsAgreement"));
        }
        [HttpPost]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ActionResult FinanceCancelProject(int number, string fExplanation, int stampTax)
        {
            var project = layer.GetProjectAgreementById(number);
            project.Status = 7;
            project.FExplanation = fExplanation;
            project.StampTax = stampTax == 1 ? true : false;
            layer.SaveProjectAgreement(project);
            util.SendEmailForProjectRejectedFinance(number);
            return Json(Url.Action("AllProjectsAgreement", "ProjectsAgreement"));
        }
        [HttpPost]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ActionResult FinanceAcceptProject(int number, string fExplanation, int stampTax)
        {
            var project = layer.GetProjectAgreementById(number);
            project.Status = 5;
            project.FExplanation = fExplanation;
            project.StampTax = stampTax == 1 ? true : false;
            layer.SaveProjectAgreement(project);
            util.SendEmailForProjectAcceptedFinance(number);
            return Json(Url.Action("AllProjectsAgreement", "ProjectsAgreement"));
        }
        [HttpPost]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult ChangeAgreementStatus(int number, int value)
        {
            var project = layer.GetProjectAgreementById(number);
            project.Status = value;
            layer.SaveProjectAgreement(project);
            return Json(new { data = value }, JsonRequestBehavior.AllowGet);
        }
    }
}