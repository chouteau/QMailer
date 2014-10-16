using System;
namespace QMailer.Web
{
	public interface IEmailTemplateService
	{
		EmailTemplate CreateCustomTemplate();
		QMailer.EmailMessage CreateEmailMessage(EmailView emailView);
		EmailView CreateEmailView(string viewName, object model = null);
		QMailer.EmailMessage GetEmailMessage(QMailer.EmailConfig emailConfig);
		System.Collections.Generic.List<EmailTemplate> GetTemplateList();
		System.Collections.Generic.IList<TemplateInfo> GetTemplateNameListByModel(string modelName);
	}
}
