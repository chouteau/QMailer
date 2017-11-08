using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using DKIM;

namespace QMailer.Dkim
{
	public class StandardEmailMessageSender : IEmailMessageSender
	{
		public StandardEmailMessageSender(Ariane.IServiceBus bus)
		{
			this.Bus = bus;
		}

		protected Ariane.IServiceBus Bus { get; private set; }

		public void Send(EmailMessage message)
		{
			try
			{
				SendInternal(message);
			}
			catch (Exception ex)
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

		private void SendInternal(EmailMessage message)
		{ 
			var mailMessage = (System.Net.Mail.MailMessage)message;

			if (message.Attachments == null
				|| message.Attachments.Count == 0)
			{
				AddDkimHeader(mailMessage);
			}

			var htmlView = AlternateView.CreateAlternateViewFromString(message.Body, null, "text/html");
			htmlView.TransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
			mailMessage.AlternateViews.Add(htmlView);

			Exception sentFailException = null;
			bool isCanceled = false;
			string senderHost = null;
			using (var sender = GlobalConfiguration.Configuration.SmtpClientFactory.Create(message.Recipients))
			{
				senderHost = sender.Host;
				using (var mailSentEvent = new System.Threading.ManualResetEvent(false))
				{
					sender.SendCompleted += (s, arg) =>
					{
						if (arg.Error != null)
						{
							sentFailException = arg.Error;
						}

						if (arg.Cancelled)
						{
							isCanceled = true;
						}

						try
						{
							if (!mailSentEvent.SafeWaitHandle.IsClosed)
							{
								mailSentEvent.Set();
							}
						}
						catch (Exception ex)
						{
							GlobalConfiguration.Configuration.Logger.Error(ex);
						}
					};

					sender.SendAsync(mailMessage, message);

					var isSent = mailSentEvent.WaitOne(30 * 1000);
					if (!isSent)
					{
						sender.SendAsyncCancel();
						isCanceled = true;
					}
					else
					{
						GlobalConfiguration.Configuration.Logger.Debug("mail for {0} sent", mailMessage.To.First().Address);
					}
				}
			}

			var sentFailQueueName = message.SentFailQueueName;
			if (string.IsNullOrWhiteSpace(sentFailQueueName))
			{
				sentFailQueueName = GlobalConfiguration.Configuration.SentFailQueueName;
			}

			if (isCanceled)
			{
				var sentFail = new SentFail();
				sentFail.Message = "Canceled";
				sentFail.Stack = "Canceled";
				sentFail.MessageId = message.MessageId;
				sentFail.Recipients = message.Recipients;
				sentFail.Subject = message.Subject;

				Bus.Send(sentFailQueueName, sentFail);

				GlobalConfiguration.Configuration.Logger.Warn("Email '{0}' sending was canceled", message.Subject);
			}
			else if (sentFailException != null)
			{
				var sentFail = new SentFail();
				sentFail.Message = sentFailException.Message;
				sentFail.Stack = sentFailException.ToString();
				sentFail.MessageId = message.MessageId;
				sentFail.Recipients = message.Recipients;
				sentFail.Subject = message.Subject;

				GlobalConfiguration.Configuration.Logger.Error(sentFailException);

				Bus.Send(sentFailQueueName, sentFail);
			}
			else if (!message.DoNotTrack)
			{
				var sentMessage = new SentMessage();
				sentMessage.Body = message.Body;
				sentMessage.MessageId = message.MessageId;
				sentMessage.Recipients = message.Recipients;
				sentMessage.Subject = message.Subject;
				sentMessage.SmtpInfo = senderHost;
				sentMessage.Sender = message.Sender;
				sentMessage.EntityId = message.EntityId;
				sentMessage.EntityName = message.EntityName;

				if (message.SenderAlias != null)
				{
					sentMessage.Sender = message.SenderAlias;
				}

				var queueName = message.SentMessageQueueName ?? GlobalConfiguration.Configuration.SentMessageQueueName;
				Bus.Send(queueName, sentMessage);
			}
		}

		public void AddDkimHeader(System.Net.Mail.MailMessage message)
		{
			if (string.IsNullOrWhiteSpace(GlobalConfiguration.Configuration.DkimPrivateKey))
			{
				return;
			}
			if (message.From.Host != GlobalConfiguration.Configuration.DkimDomain)
			{
				return;
			}

			try
			{
				var privateKey = DKIM.PrivateKeySigner.Create(GlobalConfiguration.Configuration.DkimPrivateKey);
				var headerToSign = new string[] { "From", "To", "Subject" };

				var domainKeySigner = new DKIM.DomainKeySigner(privateKey,
									GlobalConfiguration.Configuration.DkimDomain,
									GlobalConfiguration.Configuration.DkimSelector,
									headerToSign);

				var dkimSigner = new DkimSigner(privateKey,
								GlobalConfiguration.Configuration.DkimDomain,
								GlobalConfiguration.Configuration.DkimSelector,
								headerToSign);


				message.DomainKeySign(domainKeySigner);
				message.DkimSign(dkimSigner);
			}
			catch (Exception ex)
			{
				ex.Data.Add("domain", GlobalConfiguration.Configuration.DkimDomain);
				ex.Data.Add("from", message.From.Address);
				foreach (var emailTo in message.To)
				{
					ex.Data.Add(string.Format("to{0}", message.To.IndexOf(emailTo)), emailTo.Address);
				}
				ex.Data.Add("subject", message.Subject);
				GlobalConfiguration.Configuration.Logger.Error(ex);
			}
		}

	}
}
