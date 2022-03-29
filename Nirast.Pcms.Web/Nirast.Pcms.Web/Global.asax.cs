using Nirast.Pcms.Web;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Nirast.Pcms.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);

            TimeZone tz = TimeZone.CurrentTimeZone;
            if (tz.StandardName == "India Standard Time" && tz.DaylightName == "India Daylight Time")
            {
                Resource.Culture = new System.Globalization.CultureInfo("en");
            }
            else
            {
                Resource.Culture = new System.Globalization.CultureInfo("en-US");
            }



            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
      

        protected void Session_Start(Object sender, EventArgs e)
        {
            TimeZone tz = TimeZone.CurrentTimeZone;
            if (tz.StandardName == "India Standard Time" && tz.DaylightName == "India Daylight Time")
            {
                Resource.Culture = new System.Globalization.CultureInfo("en");                
            }
            else
            {
                Resource.Culture = new System.Globalization.CultureInfo("en-US");              
            }
        }
    }
}
