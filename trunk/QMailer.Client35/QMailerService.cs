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

		public EmailConfig CreateEmailConfig()
		{
			var httpClient = new RestSharpClient();
			var r = httpClient.CreateRequest("api/qmailer/createemailconfig", RestSharp.Method.GET);
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

		public Dictionary<string, string> GetTemplateListByModel(string modelName)
		{
			var httpClient = new RestSharpClient();
			var r = httpClient.CreateRequest("api/qmailer/templatelistbymodel/" + modelName, RestSharp.Method.GET);
			var result = httpClient.Execute<Dictionary<string, string>>(r);
			return result;
		}
    }
}
