using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GamingSessionApp.Startup))]
namespace GamingSessionApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
