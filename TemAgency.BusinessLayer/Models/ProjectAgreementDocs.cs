using System;

namespace TemAgency.BusinessLayer.Models
{
    public class ProjectAgreementDocs
    {
        public int Id { get; set; }
        public int ProjectAgreementId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
        public byte[] DocumentIcon { get; set; }
        public DateTime DocumentDate { get; set; }
    }
}
