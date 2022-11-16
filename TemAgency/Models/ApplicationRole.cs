using Microsoft.AspNet.Identity.EntityFramework;
namespace TemAgency.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string name) : base(name) { }
    }
}