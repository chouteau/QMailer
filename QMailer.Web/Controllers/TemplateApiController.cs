using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace QMailer.Web.Controllers
{
	[System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
	[RoutePrefix("api/qmailer")]
	public class EmailTemplateApiController : ApiController
	{
		public EmailTemplateApiController(EmailTemplateService emailTemplateService)
		{
			EmailTemplateService = emailTemplateService;
		}

		protected EmailTemplateService EmailTemplateService { get; private set; }

		[Route("ping")]
		public object GetPing()
		{
			return new
			{
				Date = DateTime.Now
			};
		}

		[Route("template/{templateName}")]
		[ActionFilters.ApiAuthorizedOperation]
		[System.Web.Http.HttpGet]
		public QMailer.EmailMessage GetTemplate(string templateName)
		{
			return EmailTemplateService.GetTemplate(templateName);
		}


		[Route("templatelist")]
		[ActionFilters.ApiAuthorizedOperation]
		[System.Web.Http.HttpGet]
		[System.Web.Http.HttpPost]
		public List<EmailTemplate> GetTemplateList()
		{
			return EmailTemplateService.GetTemplateList();
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

		[ActionFilters.ApiAuthorizedOperation]
		[Route("templatelistbymodel/{model}")]
		[HttpGet]
		public IDictionary<string, string> GetTemplateListByModel(string model)
		{
			return EmailTemplateService.GetTemplateNameListByModel(model);
		}

	}
}
