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
            EmailTemplateService emailTemplateService,
			Ariane.IServiceBus bus,
			ILogger logger
			)
		{
			this.EmailTemplateService = emailTemplateService;
			this.Bus = bus;
			this.Logger = logger;
		}

		protected EmailTemplateService EmailTemplateService { get; private set; }
		protected Ariane.IServiceBus Bus { get; private set; }
		protected ILogger Logger { get; private set; }

		public override void ProcessMessage(EmailConfig message)
		{
            ProcessEmailMessage(message);
        }

		void ProcessEmailMessage(EmailConfig message)
		{
			Logger.Debug("Create template email {0},{1},{2}", message.EmailName, message.Model, message.AssemblyQualifiedTypeNameModel);
			string messageId = Guid.NewGuid().ToString();

			object model = message.Model;
			if (message.Model != null 
				&& message.Model.GetType().AssemblyQualifiedName != message.AssemblyQualifiedTypeNameModel)
			{
				var modelType = Type.GetType(message.AssemblyQualifiedTypeNameModel);
				model = Newtonsoft.Json.JsonConvert.DeserializeObject(message.Model.ToString(), modelType);
				Logger.Debug("Email model {0},{1},{2}", message.EmailName, model, modelType);
			}
			
			dynamic emailView = EmailTemplateService.CreateEmailView(message.EmailName, model);
			emailView.MessageId = messageId;
			dynamic template = null;
			try
			{
				template = EmailTemplateService.CreateEmailMessage(emailView);
			}
			catch(Exception ex)
			{
				Logger.Error(ex);
				return;
			}

			template.Recipients.AddRange(message.Recipients);
			template.Headers.Add(new EmailMessageHeader() { Name = "X-Mailer", Value = "QMailer" });
			template.Headers.Add(new EmailMessageHeader() { Name = "X-Mailer-MID", Value = messageId });

			template.MessageId = messageId;

			Logger.Info("Enqueue email {0} to {1}", message.EmailName, message.Recipients.First(i => i.SendingType == EmailSendingType.To).Address);
			Bus.Send(GlobalConfiguration.Configuration.SendEmailQueueName, template);
		}
	}
}
