using System;

namespace TemAgency.BusinessLayer.Models
{
    public class ProjectLineSteps
    {
        public int Id { get; set; }
        public int ProjectLineId { get; set; }
        public int Status { get; set; }
        public DateTime StepDate { get; set; }
        public string Explanation { get; set; }
        public string Link { get; set; }
        public double Price { get; set; }
    }
}
