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
		internal ILogger Logger { get; set; }

		public EmailConfig CreateEmailConfig(string messageId)
		{
			var result = new EmailConfig();
			result.MessageId = messageId;
			return result;
		}

		public void SendAsync(EmailConfig emailConfig)
		{
			Logger.Info("Prepare email {0} to {1}", emailConfig.EmailName, emailConfig.Recipients.First(i => i.SendingType == EmailSendingType.To).Address);
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
		}

		internal void Send(EmailMessage message)
		{
			var mailMessage = (System.Net.Mail.MailMessage)message;

			var htmlView = AlternateView.CreateAlternateViewFromString(message.Body, null, "text/html");
			htmlView.TransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
			mailMessage.AlternateViews.Add(htmlView);

			AddDkimHeader(mailMessage);

			using (var sender = CreateSmtpClient())
			{
				Logger.Debug("Send email to : {0} with subject {1}", mailMessage.To.First().Address, mailMessage.Subject);
				sender.SendMailAsync(mailMessage).ContinueWith(task =>
				{
					if (task.IsFaulted)
					{
						task.Exception.Data.Add("MessageId", message.MessageId);
						Logger.Error(task.Exception);

						var sentFail = new SentFail();
						sentFail.Message = task.Exception.Message;
						sentFail.Stack = task.Exception.ToString();
						sentFail.MessageId = message.MessageId;
						sentFail.Recipients = message.Recipients;
						sentFail.Subject = message.Subject;

						Bus.Send(GlobalConfiguration.Configuration.SentMessageQueueName, sentFail);
					}
					else
					{
						var sentMessage = new SentMessage();
						sentMessage.Body = message.Body;
						sentMessage.MessageId = message.MessageId;
						sentMessage.Recipients = message.Recipients;
						sentMessage.Subject = message.Subject;
						sentMessage.SmtpInfo = sender.Host;

						Bus.Send(GlobalConfiguration.Configuration.SentFailQueueName, sentMessage);
					}
				}).Wait(10 * 1000);
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
				var domainKeySigner = new DKIM.DomainKeySigner(privateKey, GlobalConfiguration.Configuration.DkimPrivateKey, GlobalConfiguration.Configuration.DkimSelector, headerToSign);

				message.DomainKeySign(domainKeySigner);

				var dkimSigner = new DkimSigner(privateKey, GlobalConfiguration.Configuration.DkimPrivateKey, GlobalConfiguration.Configuration.DkimSelector, headerToSign);
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
				Logger.Error(ex);
			}
		}

	}
}
