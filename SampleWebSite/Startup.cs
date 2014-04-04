using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SampleWebSite.Startup))]
namespace SampleWebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
