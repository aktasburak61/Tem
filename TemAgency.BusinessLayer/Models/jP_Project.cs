using System.Collections.Generic;

namespace TemAgency.BusinessLayer.Models
{
    public class jP_Project
    {
        public int internal_Reference { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public ResponsibleRef responsibleRef { get; set; }
        public OrgUnitReference orgUnitReference { get; set; }
        public Activities activities { get; set; }
        public class ResponsibleRef
        {
            public int reference { get; set; }
            public string responsible_Code { get; set; }
        }
        public class OrgUnitReference
        {
            public int reference { get; set; }
            public string orgUnit_Code { get; set; }
            public int orgUnit_DomainId { get; set; }
            public OrgUnit_FirmRef orgUnit_FirmRef { get; set; }
        }
        public class OrgUnit_FirmRef
        {
            public int reference { get; set; }
            public int orgUnit_Firm_Companynr { get; set; }
            public int orgUnit_Firm_DomainId { get; set; }
        }
        public class Activities
        {
            public List<Items> items { get; set; }
        }
        public class Items
        {
            public string code { get; set; }
            public string name { get; set; }
            public double actQuantity { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
        }

    }
}
