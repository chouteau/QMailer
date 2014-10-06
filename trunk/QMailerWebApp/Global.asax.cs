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
			QMailer.GlobalConfiguration.Configuration.FromEmail = "test@email.com";
			QMailer.GlobalConfiguration.Configuration.FromName = "Test NAme";
			QMailer.QMailerService.Current.Start();
        }

		protected void Application_End()
		{
			QMailer.QMailerService.Current.Stop();
		}
    }
}
