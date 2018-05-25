using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.MailJet
{
	public class MailJetSender : QMailer.IEmailMessageSender
	{
		public MailJetSender(Ariane.IServiceBus bus)
		{
			this.Bus = bus;
			this.PublicKey = System.Configuration.ConfigurationManager.AppSettings["MailJetPublicKey"];
			this.PrivateKey = System.Configuration.ConfigurationManager.AppSettings["MailJetPrivateKey"];
		}

		internal Ariane.IServiceBus Bus { get; set; }
		public string PublicKey { get; set; }
		public string PrivateKey { get; set; }

		public void Send(EmailMessage message)
		{
			var client = new MailJetClient(PublicKey, PrivateKey);
			Models.SentMessageData result = null;

			try
			{
				result = client.SendMessage(message);
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

			if (result == null)
			{
				return;
			}

			if (!message.DoNotTrack)
			{
				var sentMessage = new SentMessage();
				sentMessage.Body = message.Body;
				sentMessage.MessageId = message.MessageId;
				sentMessage.Recipients = message.Recipients;
				sentMessage.Subject = message.Subject;
				sentMessage.SmtpInfo = "mailjetapi";
				sentMessage.Sender = message.Sender;
				sentMessage.EntityId = message.EntityId;
				sentMessage.EntityName = message.EntityName;

				if (message.SenderAlias != null)
				{
					sentMessage.Sender = message.SenderAlias;
				}

				var queueName = message.SentMessageQueueName ?? GlobalConfiguration.Configuration.SentMessageQueueName;
				if (message.SentMessage == null)
				{
					Bus.Send(queueName, sentMessage);
				}
				else
				{
					Bus.Send(queueName, message.SentMessage);
				}
			}

			//foreach (var item in result.Sent)
			//{
			//	var pending = new PendingMessage();
			//	pending.MessageId = message.MessageId;
			//	pending.MailJetMessageId = item.MessageID;
			//	pending.Email = item.Email;
			//	pending.SendDate = DateTime.Now;
			//	pending.Subject = message.Subject;
			//	pending.Recipients = message.Recipients;

			//	StatusChecker.Current.Add(pending);
			//}
		}
	}
}
