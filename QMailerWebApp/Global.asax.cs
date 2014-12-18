using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace QMailerWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
			var container = new Microsoft.Practices.Unity.UnityContainer();
			QMailer.Web.QMailerConfig.Configure(container);
			QMailer.GlobalConfiguration.Configuration.FullUrl = "http://localhost";
			QMailer.GlobalConfiguration.Configuration.SenderEmail = "test@email.com";
			QMailer.GlobalConfiguration.Configuration.SenderName = "Test NAme";
			QMailer.QMailerService.Start();
        }

		protected void Application_End()
		{
			QMailer.QMailerService.Stop();
		}
    }
}
