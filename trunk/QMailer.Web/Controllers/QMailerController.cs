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
		public QMailerController(ICacheService cacheService)
		{
			this.EmailTemplateService = QMailer.GlobalConfiguration.Configuration.DependencyResolver.GetService<QMailer.Web.EmailTemplateService>();
			this.CacheService = cacheService;
		}

		protected QMailer.Web.EmailTemplateService EmailTemplateService { get; private set; }
		protected ICacheService CacheService { get; private set; }

		public ActionResult PreviewTemplate(string id)
		{
			dynamic emailView = EmailTemplateService.CreateEmailView(id, null);
			emailView.MessageId = Guid.NewGuid().ToString();
			var template = EmailTemplateService.CreateEmailMessage(emailView);

			return Content(template.Body as string, "text/html");
		}

		public ActionResult Preview(string id)
		{
			var body = CacheService[id];
			if (body == null)
			{
				return new EmptyResult();
			}
			return Content((string)body, "text/html");
		}
	}
}
