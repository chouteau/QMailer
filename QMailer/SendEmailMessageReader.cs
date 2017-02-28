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
		public SendEmailMessageReader(IEmailMessageSender sender)
		{
			this.EmailMessageSender = sender;
		}

		protected IEmailMessageSender EmailMessageSender { get; private set; }

		public override void ProcessMessage(EmailMessage message)
		{
			EmailMessageSender.Send(message);
		}
	}
}
