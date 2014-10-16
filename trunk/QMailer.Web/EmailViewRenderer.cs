using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;

namespace QMailer.Web
{
    /// <summary>
    /// Renders <see cref="Email"/> view's into raw strings using the MVC ViewEngine infrastructure.
    /// </summary>
	public class EmailViewRenderer : IEmailViewRenderer
	{
		// StubController so we can create a ControllerContext.
		class StubController : Controller { }

		readonly ViewEngineCollection viewEngines;
		readonly string urlHostName;
		readonly string emailViewDirectoryName;

		public EmailViewRenderer(ViewEngineCollection viewEngines, string hostName, string virtualPath)
		{
			this.viewEngines = viewEngines;
			this.urlHostName = hostName ?? "http://localhost";
			this.emailViewDirectoryName = virtualPath;
		}

		public string Render(EmailView email, string viewName = null)
		{
			viewName = viewName ?? email.ViewName;
			var controllerContext = CreateControllerContext();
			var view = CreateView(viewName, controllerContext);
			var viewOutput = RenderView(view, email.ViewData, controllerContext);
			return viewOutput;
		}

		ControllerContext CreateControllerContext()
		{
			var httpContext = new EmailHttpContext(urlHostName);
			var routeData = GetRouteDataFromHttpContext() ?? new RouteData();
			routeData.Values["controller"] = emailViewDirectoryName;
			var requestContext = new RequestContext(httpContext, routeData);
			return new ControllerContext(requestContext, new StubController());
		}

		IView CreateView(string viewName, ControllerContext controllerContext)
		{
			var result = viewEngines.FindView(controllerContext, viewName, null);
			if (result.View != null)
				return result.View;

			throw new Exception(
				"Email view not found for " + viewName +
				". Locations searched:" + Environment.NewLine +
				string.Join(Environment.NewLine, result.SearchedLocations)
			);
		}

		string RenderView(IView view, ViewDataDictionary viewData, ControllerContext controllerContext)
		{
			using (var writer = new StringWriter())
			{
				var viewContext = new ViewContext(controllerContext, view, viewData, new TempDataDictionary(), writer);
				view.Render(viewContext, writer);
				return writer.GetStringBuilder().ToString();
			}
		}

		RouteData GetRouteDataFromHttpContext()
		{
			if (HttpContext.Current == null) return null;
			var wrapper = new HttpContextWrapper(HttpContext.Current);
			return RouteTable.Routes.GetRouteData(wrapper);
		}

		Uri GetUrlFromHttpContext()
		{
			if (HttpContext.Current == null) return null;
			return HttpContext.Current.Request.Url;
		}

		Uri DefaultUrlRatherThanNull()
		{
			return new Uri("http://localhost");
		}
	}
}
