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
		public SendEmailMessageReader(ILogger logger)
		{
			this.Logger = logger;
		}

		protected ILogger Logger { get; private set; }

		public override void ProcessMessage(EmailMessage message)
		{
			Logger.Debug("receive message for send by email");
			EmailerService.Current.Send(message);
		}
	}
}
