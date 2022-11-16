using Microsoft.Owin;
using Owin;
[assembly: OwinStartupAttribute(typeof(TemAgency.StartUp))]
namespace TemAgency
{
    public partial class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
