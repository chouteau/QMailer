using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;

using Ariane;

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

			using (var sender = GetCurrentSmtpClient())
			{
				Logger.Debug("Send email to : {0} with subject {1}", mailMessage.To.First().Address, mailMessage.Subject);
				try
				{
					sender.Send(mailMessage);
				}
				catch(Exception ex)
				{
					ex.Data.Add("MessageId", message.MessageId);
					Logger.Error(ex);
				}
			}
		}

		private System.Net.Mail.SmtpClient GetCurrentSmtpClient()
		{
			var client = new System.Net.Mail.SmtpClient();
			client.SendCompleted += new SendCompletedEventHandler(m_SmtpClient_SendCompleted);

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

		void m_SmtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				Logger.Error(e.Error);
			}
		}
	}
}
