using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using QMailer;

namespace QMailerWebApp.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

		[HttpPost]
		public ActionResult Contact(string templateName)
		{
			var emailConfig = QMailer.EmailerService.Current.CreateEmailConfig();
			emailConfig.SetView(templateName)
				.AddRecipient(new QMailer.EmailAddress() { Address = "test@test.com", SendingType = EmailSendingType.To });

			EmailerService.Current.SendAsync(emailConfig);

			return View();
		}
	}
}