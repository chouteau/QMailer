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
	public class EmailerService
	{
		private static Lazy<EmailerService> m_LazyInstance = new Lazy<EmailerService>(InitializeService, true);

		private EmailerService()
		{
		}

		public static EmailerService Current
		{
			get
			{
				return m_LazyInstance.Value;
			}
		}

		internal Ariane.IServiceBus Bus { get; private set; }
		internal ILogger Logger { get; private set; }

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

		private static EmailerService InitializeService()
		{
			if (GlobalConfiguration.Configuration.DependencyResolver == null)
			{
				var container = new Microsoft.Practices.Unity.UnityContainer();
				GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
			}
			var bus = GlobalConfiguration.Configuration.DependencyResolver.GetService<Ariane.IServiceBus>();

			var emailerService = new EmailerService();
			emailerService.Bus = bus;
			emailerService.Logger = new DiagnosticsLogger();

			return emailerService;
		}

	}
}
