using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QMailer
{
	public class SendEmailMessageReader : Ariane.MessageReaderBase<EmailMessage>
	{
		public SendEmailMessageReader(ILogger logger,
			Ariane.IServiceBus bus
			)
		{
			this.Logger = logger;
			this.Bus = bus;
		}

		protected ILogger Logger { get; private set; }
		protected Ariane.IServiceBus Bus { get; private set; }

		public override void ProcessMessage(EmailMessage message)
		{
			Logger.Debug("receive message for send by email");
			try
			{
				((EmailerService)QMailerService.Current).Send(message);
			}
			catch(Exception ex)
			{
				var sentFail = new SentFail();
				sentFail.Message = ex.Message;
				sentFail.Stack = ex.ToString();
				sentFail.MessageId = message.MessageId;
				sentFail.Recipients = message.Recipients;
				sentFail.Subject = message.Subject;

				Bus.Send(GlobalConfiguration.Configuration.SentMessageQueueName, sentFail);
			}
		}
	}
}
