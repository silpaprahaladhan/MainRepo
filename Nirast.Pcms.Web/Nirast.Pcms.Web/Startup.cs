using Microsoft.Owin;
using Nirast.Pcms.Web;
using Owin;

[assembly: OwinStartupAttribute(typeof(Startup))]
namespace Nirast.Pcms.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.MapSignalR();
        }
    }
}





