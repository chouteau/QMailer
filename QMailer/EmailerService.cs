using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;

using Ariane;

using DKIM;

namespace QMailer
{
	class EmailerService : QMailer.IEmailerService
	{

		internal EmailerService()
		{
		}

		internal Ariane.IServiceBus Bus { get; set; }

		public EmailConfig CreateEmailConfig(string messageId = null)
		{
			var result = new EmailConfig();
			result.MessageId = messageId ?? Guid.NewGuid().ToString();
			return result;
		}

		public void SendAsync(EmailConfig emailConfig)
		{
			var queueName = emailConfig.EmailBodyRequestedQueueName ?? GlobalConfiguration.Configuration.EmailBodyRequestedQueueName;
			Bus.Send(GlobalConfiguration.Configuration.EmailBodyRequestedQueueName, emailConfig);
		}

		public void ReplaceBusService(Ariane.IServiceBus bus)
		{
			Bus = bus;
		}

		public void Stop()
		{
			Bus.StopReading();
		}

		public void Start()
		{
			Bus.StartReading();
			GlobalConfiguration.Configuration.Logger.Info("Service started");
		}

		internal void Send(EmailMessage message)
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
			using (var sender = CreateSmtpClient())
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

							if (!mailSentEvent.SafeWaitHandle.IsClosed)
							{
								mailSentEvent.Set();
							}
						};

					sender.SendAsync(mailMessage, message);

					var isSent = mailSentEvent.WaitOne(30 * 1000);
					if (!isSent)
					{
						sender.SendAsyncCancel();
						isCanceled = true;
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

		private System.Net.Mail.SmtpClient CreateSmtpClient()
		{
			var client = new System.Net.Mail.SmtpClient();

			// For Tests
			if (client.DeliveryMethod == System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory
				&& client.PickupDirectoryLocation.StartsWith(@".\"))
			{
				var path = System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location);
				path = System.IO.Path.Combine(path, "emails");
				if (!System.IO.Directory.Exists(path))
				{
					System.IO.Directory.CreateDirectory(path);
				}
				client.PickupDirectoryLocation = path;
			}

			return client;
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
