using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace QMailer.Web.Controllers
{
	[RoutePrefix("api/qmailer")]
	public class QMailerApiController : ApiController
	{
		public QMailerApiController(
			EmailTemplateService emailTemplateService,
			Ariane.IServiceBus bus,
			ICacheService cacheService
			)
		{
			this.EmailTemplateService = emailTemplateService;
			this.Bus = bus;
			this.CacheService = cacheService;
		}

		protected EmailTemplateService EmailTemplateService { get; private set; }
		protected Ariane.IServiceBus Bus { get; private set; }
		protected ICacheService CacheService { get; private set; }

		[Route("ping")]
		public object GetPing()
		{
			return new
			{
				Date = DateTime.Now
			};
		}

		[ActionFilters.ApiAuthorizedOperation]
		[System.Web.Http.HttpGet]
		[Route("createemailconfig/{messageId}")]
		public EmailConfig CreateEmailConfig(string messageId)
		{
			var emailConfig = QMailerService.Current.CreateEmailConfig(messageId);
			return emailConfig;
		}

		[Route("previewkey")]
		[ActionFilters.ApiAuthorizedOperation]
		[System.Web.Http.HttpPost]
		public object GetPreviewKey(EmailConfig emailConfig)
		{
			var emailMessage = EmailTemplateService.GetEmailMessage(emailConfig);
			if (emailMessage != null)
			{
				CacheService.Add(emailConfig.MessageId, emailMessage.Body, DateTime.Now.AddHours(1));
			}
			return new
			{
				MessageId = emailConfig.MessageId
			};
		}

		[ActionFilters.ApiAuthorizedOperation]
		[System.Web.Http.HttpPost]
		[Route("sendemailconfig")]
		public void SendAsync(EmailConfig emailConfig)
		{
			QMailerService.Current.SendAsync(emailConfig);
		}

		[ActionFilters.ApiAuthorizedOperation]
		[System.Web.Http.HttpPost]
		[Route("sendemailmessage")]
		public void SendAsync(EmailMessage emailMessage)
		{
			Bus.Send(GlobalConfiguration.Configuration.SendEmailQueueName, emailMessage);
		}

		[Route("emailmessage")]
		[ActionFilters.ApiAuthorizedOperation]
		[System.Web.Http.HttpPost]
		public EmailMessage GetEmailMessage(EmailConfig emailConfig)
		{
			return EmailTemplateService.GetEmailMessage(emailConfig);
		}

		[Route("templatelist")]
		[ActionFilters.ApiAuthorizedOperation]
		[System.Web.Http.HttpGet]
		[System.Web.Http.HttpPost]
		public List<EmailTemplate> GetTemplateList()
		{
			return EmailTemplateService.GetTemplateList();
		}

		[ActionFilters.ApiAuthorizedOperation]
		[Route("templatelistbymodel/{model}")]
		[HttpGet]
		public IList<TemplateInfo> GetTemplateListByModel(string model)
		{
			return EmailTemplateService.GetTemplateNameListByModel(model);
		}

		[Route("createcustomtemplate")]
		[ActionFilters.ApiAuthorizedOperation]
		[System.Web.Http.HttpGet]
		[System.Web.Http.HttpPost]
		public EmailTemplate CreateCustomTemplate()
		{
			return EmailTemplateService.CreateCustomTemplate();
		}

		[Route("deletetemplate")]
		[ActionFilters.ApiAuthorizedOperation]
		[System.Web.Http.HttpPost]
		[System.Web.Http.HttpDelete]
		public bool DeleteTemplate(EmailTemplate template)
		{
			EmailTemplateService.DeleteTemplate(template);
			return true;
		}

		[Route("savetemplate")]
		[ActionFilters.ApiAuthorizedOperation]
		[System.Web.Http.HttpPost]
		[System.Web.Http.HttpPut]
		public bool SaveTemplate(EmailTemplate template)
		{
			EmailTemplateService.SaveTemplate(template);
			return true;
		}



	}
}
