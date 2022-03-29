using System;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Nirast.Pcms.Api.Startup))]

namespace Nirast.Pcms.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);
            ConfigureAuth(app);
        }
    }
}
