using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace TemAgency.BusinessLayer
{
    public class DataLayer
    {
        public DataLayer()
        {
        }
        JPLATFORMAEntities jcontext = new JPLATFORMAEntities();
        TEMAGENCYEntities tcontext = new TEMAGENCYEntities();
        public Ntl_Projects GetProjectById(int number)
        {
            var project = tcontext.Ntl_Projects.Where(x => x.Id == number).FirstOrDefault();
            return project;
        }
        public Ntl_ProjectsAgreement GetProjectAgreementById(int number)
        {
            tcontext = new TEMAGENCYEntities();
            var projectAgreement = tcontext.Ntl_ProjectsAgreement.Where(x => x.Id == number).FirstOrDefault();
            return projectAgreement;
        }
        public Ntl_Sector GetSectorById(int number)
        {
            var sector = tcontext.Ntl_Sector.Where(x => x.Id == number).FirstOrDefault();
            return sector;
        }
        public Ntl_SubSector GetSubSectorById(int number)
        {
            var subSector = tcontext.Ntl_SubSector.Where(x => x.Id == number).FirstOrDefault();
            return subSector;
        }
        public U_001_ANLYDIMENSIONS GetInfluencerById(int number)
        {
            var influencer = jcontext.U_001_ANLYDIMENSIONS.Where(x => x.LOGICALREF == number).FirstOrDefault();
            return influencer;
        }
        public U_001_AUXCODES GetPlatformById(int number)
        {
            var platform = jcontext.U_001_AUXCODES.Where(x => x.LOGICALREF == number).FirstOrDefault();
            return platform;
        }
        public U_001_ANLYDIMTYPES GetDocumentTypeById(int number)
        {
            var documentType = jcontext.U_001_ANLYDIMTYPES.Where(x => x.LOGICALREF == number).FirstOrDefault();
            return documentType;
        }
        public U_001_ARPS GetCustomerNameById(int number)
        {
            var customerName = jcontext.U_001_ARPS.Where(x => x.LOGICALREF == number).FirstOrDefault();
            return customerName;
        }
        public S_ORGUNITS GetCompanyById(int number)
        {
            var company = jcontext.S_ORGUNITS.Where(x => x.LOGICALREF == number).FirstOrDefault();
            return company;
        }
        public Ntl_Brand GetBrandById(int number)
        {
            var brand = tcontext.Ntl_Brand.Where(x => x.Id == number).FirstOrDefault();
            return brand;
        }
        public string GetNextProjectCode()
        {
            string lastNr = tcontext.Ntl_Projects.OrderByDescending(x => x.Id).Select(y => y.ProjectCode).FirstOrDefault();
            if (string.IsNullOrEmpty(lastNr))
            {
                lastNr = "Prj-000001";
            }
            else
            {
                lastNr = "Prj-" + (Convert.ToInt32(lastNr.Split('-')[1]) + 1).ToString().PadLeft(6, '0');
            }
            return lastNr;
        }
        public string GetNextSectorCode()
        {
            string lastCode = tcontext.Ntl_Sector.OrderByDescending(x => x.Code).Select(y => y.Code).FirstOrDefault();
            if (string.IsNullOrEmpty(lastCode))
            {
                lastCode = "00001";
            }
            else
            {
                lastCode = (Convert.ToInt32(lastCode) + 1).ToString().PadLeft(5, '0');
            }
            return lastCode;
        }
        public string GetNextSubSectorCode()
        {
            string lastCode = tcontext.Ntl_SubSector.OrderByDescending(x => x.Code).Select(y => y.Code).FirstOrDefault();
            if (string.IsNullOrEmpty(lastCode))
            {
                lastCode = "00001";
            }
            else
            {
                lastCode = (Convert.ToInt32(lastCode) + 1).ToString().PadLeft(5, '0');
            }
            return lastCode;
        }
        public void SaveProject(Ntl_Projects ntl_project)
        {
            if (ntl_project.Id == 0)
            {
                tcontext.Ntl_Projects.Add(ntl_project);
            }
            else
            {
                tcontext.Entry(ntl_project).State = EntityState.Modified;
            }
            tcontext.SaveChanges();
            foreach (var line in ntl_project.Ntl_ProjectLines)
            {
                if (line.Id == 0)
                {
                    line.ProjectId = ntl_project.Id;
                    tcontext.Ntl_ProjectLines.Add(line);
                }
                else
                {
                    tcontext.Entry(line).State = EntityState.Modified;
                }
                tcontext.SaveChanges();
                foreach (var step in line.Ntl_ProjectLineSteps)
                {
                    if (step.Id == 0)
                    {
                        step.ProjectLineId = line.Id;
                        tcontext.Ntl_ProjectLineSteps.Add(step);
                    }
                    else
                    {
                        tcontext.Entry(step).State = EntityState.Modified;

                    }
                    tcontext.SaveChanges();
                }
            }
        }
        public void SaveProjectAgreement(Ntl_ProjectsAgreement ntl_ProjectsAgreement)
        {
            if (ntl_ProjectsAgreement.Id == 0)
            {
                tcontext.Ntl_ProjectsAgreement.Add(ntl_ProjectsAgreement);
            }
            else
            {
                tcontext.Entry(ntl_ProjectsAgreement).State = EntityState.Modified;
            }
            tcontext.SaveChanges();
        }
        public void SaveProjectLineSteps(List<Ntl_ProjectLineSteps> ntl_ProjectLineSteps)
        {
            foreach (var item in ntl_ProjectLineSteps)
            {
                tcontext.Entry(item).State = EntityState.Modified;
            }
            tcontext.SaveChanges();
        }
        public void SaveSector(Ntl_Sector sector)
        {
            if (sector.Id == 0)
            {
                tcontext.Ntl_Sector.Add(sector);
            }
            else
            {
                tcontext.Entry(sector).State = EntityState.Modified;
            }
            tcontext.SaveChanges();
        }
        public void SaveSubSector(Ntl_SubSector subSector)
        {
            if (subSector.Id == 0)
            {
                tcontext.Ntl_SubSector.Add(subSector);
            }
            else
            {
                tcontext.Entry(subSector).State = EntityState.Modified;
            }
            tcontext.SaveChanges();
        }
        public void SaveBrand(Ntl_Brand brand)
        {
            if (brand.Id == 0)
            {
                tcontext.Ntl_Brand.Add(brand);
            }
            else
            {
                tcontext.Entry(brand).State = EntityState.Modified;
            }
            tcontext.SaveChanges();
        }
        public void DeleteSector(Ntl_Sector sector)
        {
            if (sector.Ntl_SubSector != null)
            {
                tcontext.Ntl_SubSector.RemoveRange(sector.Ntl_SubSector);
            }
            tcontext.Ntl_Sector.Remove(sector);
            tcontext.SaveChanges();
        }
        public void DeleteSubSector(Ntl_SubSector subSector)
        {
            tcontext.Ntl_SubSector.Remove(subSector);
            tcontext.SaveChanges();
        }
        public void DeleteBrand(Ntl_Brand brand)
        {
            tcontext.Ntl_Brand.Remove(brand);
            tcontext.SaveChanges();
        }
        public Ntl_ProjectDocs GetDocumentById(int documentId)
        {
            var doc = tcontext.Ntl_ProjectDocs.FirstOrDefault(x => x.Id == documentId);
            return doc;
        }
        public Ntl_ProjectAgreementDocs GetDocumentProjectsAgreementById(int documentId)
        {
            var doc = tcontext.Ntl_ProjectAgreementDocs.FirstOrDefault(x => x.Id == documentId);
            return doc;
        }
        public string GetProjectNrById(int projectId)
        {
            var project = tcontext.Ntl_Projects.Where(z => z.Id == projectId).Select(y => y.ProjectCode).FirstOrDefault();
            return project;
        }
        public void SaveProjectDoc(Ntl_ProjectDocs doc)
        {
            tcontext.Ntl_ProjectDocs.Add(doc);
            tcontext.SaveChanges();
        }
        public void SaveProjectAgreementDoc(Ntl_ProjectAgreementDocs doc)
        {
            if (doc.ProjectAgreementId == 0)
            {
                doc.ProjectAgreementId = null;
                tcontext.Ntl_ProjectAgreementDocs.Add(doc);
            }
            else
            {
                tcontext.Ntl_ProjectAgreementDocs.Add(doc);
            }
            tcontext.SaveChanges();
        }
        public bool DeleteDocumentById(int documentId)
        {
            try
            {
                tcontext.Ntl_ProjectDocs.Remove(GetDocumentById(documentId));
                return tcontext.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteDocumentProjectsAgreementById(int documentId)
        {
            try
            {
                tcontext.Ntl_ProjectAgreementDocs.Remove(GetDocumentProjectsAgreementById(documentId));
                return tcontext.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public double TotalTransfered(int number)
        {
            double total = tcontext.Ntl_ProjectLineSteps.Where(x => x.ProjectLineId == number && x.Status == 3).Sum(x => x.Price) ?? 0;
            return total;
        }
        public int UncompletedSteps(int number)
        {
            int steps = tcontext.Ntl_ProjectLineSteps.Where(x => x.ProjectLineId == number && x.Status != 3).Count();
            return steps;
        }
        public Ntl_ProjectLines GetProjectLineById(int number)
        {
            var line = tcontext.Ntl_ProjectLines.FirstOrDefault(x => x.Id == number);
            return line;
        }
        public List<Ntl_ProjectAgreementDocs> GetNullProjectAgreementDocs()
        {
            var docs = tcontext.Ntl_ProjectAgreementDocs.Where(x => x.ProjectAgreementId == null).ToList();
            return docs;
        }
        public string GetStatusStr(int status)
        {
            string statusStr = "";
            switch (status)
            {
                case 1:
                    statusStr = "Proje Oluşturuldu";
                    break;
                case 2:
                    statusStr = "Bütçe Gönderilecek";
                    break;
                case 3:
                    statusStr = "Bütçe Gönderildi";
                    break;
                case 4:
                    statusStr = "Onay Bekliyor";
                    break;
                case 5:
                    statusStr = "Brief Bekleniyor";
                    break;
                case 6:
                    statusStr = "Rapor Hazırlanıyor";
                    break;
                case 7:
                    statusStr = "İçerikler Hazırlanıyor";
                    break;
                case 8:
                    statusStr = "Yayınlanacak";
                    break;
                case 9:
                    statusStr = "İçerikler Tamamlandı";
                    break;
                case 10:
                    statusStr = "Revizeler Bekleniyor";
                    break;
                case 11:
                    statusStr = "Fatura Kesilecek";
                    break;
                case 12:
                    statusStr = "Vade Bekliyor";
                    break;
                case 13:
                    statusStr = "İptal";
                    break;
                case 14:
                    statusStr = "Proje Kapandı";
                    break;
                default:
                    break;
            }
            return statusStr;
        }
        public AspNetUsers GetUserByUserName(string userName)
        {
            var user = tcontext.AspNetUsers.FirstOrDefault(x => x.UserName == userName);
            return user;
        }
        public AspNetUsers GetUserByEmail(string email)
        {
            var user = tcontext.AspNetUsers.FirstOrDefault(x => x.Email == email);
            return user;
        }
        public AspNetRoles GetRoleById(string Id)
        {
            var role = tcontext.AspNetRoles.FirstOrDefault(x => x.Id == Id);
            return role;
        }
        public AspNetUsers GetUserById(string number)
        {
            var user = tcontext.AspNetUsers.FirstOrDefault(x => x.Id == number);
            return user;
        }
        public List<AspNetRoles> GetRoles()
        {
            var roles = tcontext.AspNetRoles.ToList();
            return roles;
        }
        public string EditUser(AspNetUsers user)
        {
            tcontext.Entry(user).State = EntityState.Modified;
            tcontext.SaveChanges();
            return user.Id;
        }
        public List<AspNetUsers> GetUsers()
        {
            return tcontext.AspNetUsers.ToList();
        }
    }
}
