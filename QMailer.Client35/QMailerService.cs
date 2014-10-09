using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QMailer.Client35
{
    public class QMailerService
    {
		public QMailerService()
		{

		}

		public EmailConfig CreateEmailConfig(string messageId)
		{
			var httpClient = new RestSharpClient();
			var r = httpClient.CreateRequest("api/qmailer/createemailconfig/" + messageId, RestSharp.Method.GET);
			var result = httpClient.Execute<EmailConfig>(r);
			return result;
		}

		public void Send(EmailConfig emailConfig)
		{
			var httpClient = new RestSharpClient();
			var r = httpClient.CreateRequest("api/qmailer/sendemailconfig", RestSharp.Method.POST);
			r.AddBody(emailConfig);
			httpClient.Execute(r);
		}

		public EmailMessage GetEmailMessage(EmailConfig emailConfig)
		{
			if (emailConfig.Model == null)
			{
				return null;
			}

			emailConfig.AssemblyQualifiedTypeNameModel = emailConfig.Model.GetType().AssemblyQualifiedName;

			var httpClient = new RestSharpClient();
			var r = httpClient.CreateRequest("api/qmailer/emailmessage", RestSharp.Method.POST);

		

			r.AddBody(emailConfig);
			var result = httpClient.Execute<EmailMessage>(r);
			return result;
		}

		public void Send(EmailMessage emailMessage)
		{
			var httpClient = new RestSharpClient();
			var r = httpClient.CreateRequest("api/qmailer/sendemailmessage", RestSharp.Method.POST);
			r.AddBody(emailMessage);
			httpClient.Execute(r);
		}

		public List<QMailer.Web.TemplateInfo> GetTemplateListByModel(string modelName)
		{
			var httpClient = new RestSharpClient();
			var r = httpClient.CreateRequest("api/qmailer/templatelistbymodel/" + modelName, RestSharp.Method.GET);
			var result = httpClient.Execute<List<QMailer.Web.TemplateInfo>>(r);
			return result;
		}

		public List<QMailer.Web.TemplateInfo> GetAllTemplateList()
		{
			throw new NotImplementedException();
		}

		public void DeleteTemplate(QMailer.Web.TemplateInfo template)
		{
			throw new NotImplementedException();
		}

		public void SaveTemplate(QMailer.Web.TemplateInfo template)
		{
			throw new NotImplementedException();
		}

		public QMailer.Web.TemplateInfo CreateCustomTemplate()
		{
			throw new NotImplementedException();
		}

		public string GetPreviewUrl(QMailer.EmailConfig config)
		{
			var httpClient = new RestSharpClient();
			var r = httpClient.CreateRequest("api/qmailer/previewkey", RestSharp.Method.POST);
			r.AddBody(config);
			var key = httpClient.Execute<string>(r);
			return string.Format("{0}/qmailer/preview/{1}", GlobalConfiguration.Configuration.ApiUrl.TrimEnd('/'), key);
		}

    }
}
