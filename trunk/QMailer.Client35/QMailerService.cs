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
			var httpClient = new RestSharpClient();
			var r = httpClient.CreateRequest("api/qmailer/emailmessage", RestSharp.Method.POST);
			r.AddBody(emailConfig);
			var result = httpClient.Execute<EmailMessage>(r);
			return result;
		}

		public void Send(EmailMessage emailMessage)
		{
			var httpClient = new RestSharpClient();
			var r = httpClient.CreateRequest("api/qmailer/emailmessage", RestSharp.Method.POST);
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
    }
}
