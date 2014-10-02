using System;
using System.Collections.Specialized;
using System.Web;
using System.Collections.Generic;
using System.Collections;

namespace QMailer.Web
{
    // Implement just enough HttpRequest junk to allow the view engine and views to work.
    // This allows the email rendering to occur on a non-web request thread, 
    // e.g. a background task.

	class EmailHttpRequest : HttpRequestBase
	{
		readonly Uri url;
		readonly NameValueCollection serverVariables = new NameValueCollection();
		readonly HttpCookieCollection cookies = new HttpCookieCollection();
		private Lazy<HttpBrowserCapabilitiesBase> browser;

		public EmailHttpRequest(string urlHostName)
		{
			url = new Uri(urlHostName);
			browser = new Lazy<HttpBrowserCapabilitiesBase>(CreateHttpBrowserCapabilities);
		}

		public override string ApplicationPath
		{
			get { return HttpRuntime.AppDomainAppVirtualPath; }
		}

		public override NameValueCollection ServerVariables
		{
			get { return serverVariables; }
		}

		public override HttpBrowserCapabilitiesBase Browser
		{
			get
			{
				return browser.Value;
			}
		}

		public override Uri Url
		{
			get { return url; }
		}

		public override bool IsLocal
		{
			get
			{
				return !url.IsAbsoluteUri;
			}
		}

		public override bool IsSecureConnection
		{
			get
			{
				return false;
			}
		}

		public override HttpCookieCollection Cookies
		{
			get
			{
				return cookies;
			}
		}

		public override string UserAgent
		{
			get
			{
				return "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.34 Safari/537.36";
			}
		}

		HttpBrowserCapabilitiesWrapper CreateHttpBrowserCapabilities()
		{
			var factory = new System.Web.Configuration.BrowserCapabilitiesFactory();
			var browserCapabilities = new HttpBrowserCapabilities();
			if (HttpContext.Current != null)
			{
				browserCapabilities.Capabilities = HttpContext.Current.Request.Browser.Capabilities;
			}
			else
			{
				browserCapabilities.Capabilities = new Hashtable 
				{ 
					{ string.Empty, UserAgent } 
				};
			}
			factory.ConfigureBrowserCapabilities(new NameValueCollection(), browserCapabilities);
			return new HttpBrowserCapabilitiesWrapper(browserCapabilities);
		}
	}
}
