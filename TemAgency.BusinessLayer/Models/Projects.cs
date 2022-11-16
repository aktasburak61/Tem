using System;
using System.Collections.Generic;

namespace TemAgency.BusinessLayer.Models
{
    public class Projects
    {
        public int Id { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public DateTime ProjectDate { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public string Origin { get; set; }
        public int BrandId { get; set; }
        public string Brand { get; set; }
        public int SectorId { get; set; }
        public string Sector { get; set; }
        public int SubSectorId { get; set; }
        public string SubSector { get; set; }
        public string Agency { get; set; }
        public int CustomerNameId { get; set; }
        public string CustomerName { get; set; }
        public int AgencyId { get; set; }
        public string AgencyName { get; set; }
        public int Status { get; set; }
        public string StatusStr { get; set; }
        public string UserId { get; set; }
        public string User { get; set; }
        public List<ProjectLines> ProjectLines { get; set; } = new List<ProjectLines>();
        public List<ProjectDocs> ProjectDocs { get; set; } = new List<ProjectDocs>();
    }
}
