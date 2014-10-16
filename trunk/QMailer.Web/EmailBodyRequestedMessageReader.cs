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
			Ariane.IServiceBus bus,
			ILogger logger
			)
		{
			this.EmailTemplateService = emailTemplateService;
			this.Bus = bus;
			this.Logger = logger;
		}

		protected IEmailTemplateService EmailTemplateService { get; private set; }
		protected Ariane.IServiceBus Bus { get; private set; }
		protected ILogger Logger { get; private set; }

		public override void ProcessMessage(EmailConfig message)
		{
            ProcessEmailMessage(message);
        }

		void ProcessEmailMessage(EmailConfig message)
		{
			var template = EmailTemplateService.GetEmailMessage(message);
			if (template != null)
			{
				Logger.Info("Enqueue email {0} to {1}", message.EmailName, template.Recipients.First(i => i.SendingType == EmailSendingType.To).Address);
				Bus.Send(GlobalConfiguration.Configuration.SendEmailQueueName, template);
			}
		}
	}
}
