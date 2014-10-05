using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace QMailer.Web.Controllers.ActionFilters 
{
	public class ApiAuthorizedOperationAttribute : System.Web.Http.Filters.ActionFilterAttribute
	{
		public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			var apiKeyHeader = actionContext.Request.Headers.FirstOrDefault(h => "apikey".Equals(h.Key, StringComparison.InvariantCultureIgnoreCase));
			if (apiKeyHeader.Value == null)
			{
				throw new System.Security.SecurityException("this service required an api key");
			}
			if (!apiKeyHeader.Value.First().Equals(GlobalConfiguration.Configuration.ApiToken))
			{
				throw new System.Security.SecurityException("this service is denied with this api key");
			}

			base.OnActionExecuting(actionContext);
		}
	}
}
