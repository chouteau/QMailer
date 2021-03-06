﻿using System;
using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Web.Instrumentation;

namespace QMailer.Web
{
    // Implement just enough HttpContext junk to allow the view engine and views to work.
    // This allows the email rendering to occur on a non-web request thread, 
    // e.g. a background task.

    class EmailHttpContext : HttpContextBase
    {
        public EmailHttpContext(string urlHostName)
        {
            items = new Hashtable();
            request = new EmailHttpRequest(urlHostName);
            response = new EmailHttpResponse();
        }

        Hashtable items;
        HttpRequestBase request;
        HttpResponseBase response;

        public override IDictionary Items { get { return items; } }
        public override HttpRequestBase Request { get { return request; } }
        public override HttpResponseBase Response { get { return response; } }
        public override Cache Cache { get { return HttpRuntime.Cache; } }
		public override HttpServerUtilityBase Server { get { return new HttpServerUtilityWrapper(HttpContext.Current.Server); } }

        public override PageInstrumentationService PageInstrumentation
        {
            get
            {
                return new PageInstrumentationService();
            }
        }

		public override object GetService(Type serviceType)
		{
			var result = System.Web.Mvc.DependencyResolver.Current.GetService(serviceType);
			return result;
		}

	}
}
