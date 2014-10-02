using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QMailer.Web.Controllers
{
	public class QMailerController : AsyncController
	{
		public QMailerController()
		{
			EmailTemplateService = QMailer.GlobalConfiguration.Configuration.DependencyResolver.GetService<QMailer.Web.EmailTemplateService>();
		}

		protected QMailer.Web.EmailTemplateService EmailTemplateService { get; private set; }

		public ActionResult Preview(string id)
		{
			dynamic emailView = EmailTemplateService.CreateEmailView(id, null);
			emailView.MessageId = Guid.NewGuid().ToString();
			var template = EmailTemplateService.CreateEmailMessage(emailView);

			return Content(template.Body as string, "text/html");
		}
	}
}
