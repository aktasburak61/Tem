//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TemAgency.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ntl_ProjectsAgreement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ntl_ProjectsAgreement()
        {
            this.Ntl_ProjectAgreementDocs = new HashSet<Ntl_ProjectAgreementDocs>();
        }
    
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public Nullable<System.DateTime> AgreementDate { get; set; }
        public Nullable<int> Company { get; set; }
        public string Explanation { get; set; }
        public string LExplanation { get; set; }
        public string FExplanation { get; set; }
        public Nullable<int> Status { get; set; }
        public string UserId { get; set; }
        public Nullable<bool> StampTax { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ntl_ProjectAgreementDocs> Ntl_ProjectAgreementDocs { get; set; }
    }
}
