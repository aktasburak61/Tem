using System;
using System.Collections.Generic;

namespace TemAgency.BusinessLayer.Models
{
    public class ProjectsAgreement
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime AgreementDate { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public string Explanation { get; set; }
        public string LExplanation { get; set; }
        public string FExplanation { get; set; }
        public int Status { get; set; }
        public string StatusStr { get; set; }
        public string UserId { get; set; }
        public string User { get; set; }
        public bool StampTax { get; set; }
        public List<ProjectAgreementDocs> ProjectAgreementDocs { get; set; } = new List<ProjectAgreementDocs>();
    }
}
