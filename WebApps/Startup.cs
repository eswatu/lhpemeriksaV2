using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebApps.Startup))]
namespace WebApps
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
