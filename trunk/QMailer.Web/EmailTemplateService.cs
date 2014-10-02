using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QMailer.Web
{
	public class EmailTemplateService 
	{
		/* public EmailTemplateService()
		{
			var emailEngines = new ViewEngineCollection() { new EmailerViewEngine() };
			this.Renderer = new EmailViewRenderer(emailEngines);
		} */

		public EmailTemplateService(IEmailViewRenderer renderer)
		{
			this.EmailParser = new EmailParser();
			this.Renderer = renderer;
		}

		protected IEmailViewRenderer Renderer { get; private set; }
		internal EmailParser EmailParser { get; private set; }

		public EmailView CreateEmailView(string viewName, object model = null)
		{
			var email = new EmailView(viewName, model);
			return email;
		}

		public EmailMessage CreateEmailMessage(EmailView emailView)
		{
			var rawEmailString = Renderer.Render(emailView);
			var result = EmailParser.CreateMailMessage(rawEmailString, emailView);
			return result;
		}

		public EmailMessage GetTemplate(string templateName)
		{
			return GetTemplate(templateName, null);
		}

		public EmailMessage GetTemplate(string templateName, object model)
		{
			dynamic emailView = CreateEmailView(templateName, model);

			var result = CreateEmailMessage(emailView);
			return result;
		}

	}
}
