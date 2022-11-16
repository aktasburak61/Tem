using System.Collections.Generic;

namespace TemAgency.BusinessLayer.Models
{
    public class ProjectLines
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public int VatRate { get; set; }
        public double Total { get; set; }
        public double VatTotal { get; set; }
        public double NetTotal { get; set; }
        public int PlatformId { get; set; }
        public string Platform { get; set; }
        public bool Live { get; set; }
        public bool IsAgency { get; set; }
        public int AgencyId { get; set; }
        public string Agency { get; set; }
        public int InfluencerId { get; set; }
        public string Influencer { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public double BuyPrice { get; set; }
        public double LineRemaining { get; set; }
        public List<ProjectLineSteps> ProjectLineSteps { get; set; } = new List<ProjectLineSteps>();
    }
}
