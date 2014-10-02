using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QMailer.Web
{
	public class EmailerViewEngine : RazorViewEngine
	{
		public EmailerViewEngine()
			: base()
		{
			var path = GlobalConfiguration.Configuration.VirtualPath;
			if (!path.StartsWith("views"))
			{
				base.ViewLocationFormats = new string[] 
				{	
					path + "/{0}.cshtml",
					path + "/{1}/{0}.cshtml",
				};
				base.PartialViewLocationFormats = new string[]
				{
					path + "/{0}.cshtml",
					path + "/{1}/{0}.cshtml",
				};
			}
		}

		protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
		{
			var nameSpace = controllerContext.Controller.GetType().Namespace;
			return base.CreatePartialView(controllerContext, partialPath.Replace("%1", nameSpace));
		}

		protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
		{
			var nameSpace = controllerContext.Controller.GetType().Namespace;
			return base.CreateView(controllerContext, viewPath.Replace("%1", nameSpace), masterPath.Replace("%1", nameSpace));
		}
			
		protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
		{
			var nameSpace = controllerContext.Controller.GetType().Namespace;
			return base.FileExists(controllerContext, virtualPath.Replace("%1", nameSpace));
		}

	}
}
