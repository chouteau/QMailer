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
		public SendEmailMessageReader(
			Ariane.IServiceBus bus
			)
		{
			this.Bus = bus;
		}

		protected Ariane.IServiceBus Bus { get; private set; }

		public override void ProcessMessage(EmailMessage message)
		{
			GlobalConfiguration.Configuration.Logger.Debug("receive message for send by email");
			try
			{
				((EmailerService)QMailerService.GetInstance()).Send(message);
			}
			catch(Exception ex)
			{
				var sentFail = new SentFail();
				sentFail.Message = ex.Message;
				sentFail.Stack = ex.ToString();
				sentFail.MessageId = message.MessageId;
				sentFail.Recipients = message.Recipients;
				sentFail.Subject = message.Subject;

				var failQueueName = message.SentFailQueueName ?? GlobalConfiguration.Configuration.SentFailQueueName;
				Bus.Send(failQueueName, sentFail);
			}
		}
	}
}
