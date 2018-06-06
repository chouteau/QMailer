using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.Web
{
	public class EmailBodyRequestedMessageReader : Ariane.MessageReaderBase<EmailConfig>
	{
		public EmailBodyRequestedMessageReader(
            IEmailTemplateService emailTemplateService,
			Ariane.IServiceBus bus
			)
		{
			this.EmailTemplateService = emailTemplateService;
			this.Bus = bus;
		}

		protected IEmailTemplateService EmailTemplateService { get; private set; }
		protected Ariane.IServiceBus Bus { get; private set; }

		public override void ProcessMessage(EmailConfig message)
		{
			var template = EmailTemplateService.GetEmailMessage(message);
			if (template != null)
			{
				var queueName = message.SendEmailQueueName ?? GlobalConfiguration.Configuration.SendEmailQueueName;
				Bus.Send(queueName, template);
			}
		}
	}
}
