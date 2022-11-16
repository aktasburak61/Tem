using System.Collections.Generic;

namespace TemAgency.BusinessLayer.Models
{
    public class ProjectAgreementMM
    {
        public ProjectsAgreement projectsAgreement { get; set; } = new ProjectsAgreement();
        public List<ProjectAgreementDocs> ProjectAgreementDocs { get; set; } = new List<ProjectAgreementDocs>();
    }
}
