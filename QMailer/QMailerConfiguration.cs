using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer
{
	public class QMailerConfiguration
	{
		public QMailerConfiguration()
		{
			EmailBodyRequestedQueueName = "emailbodyrequested";
			SendEmailQueueName = "sendemail";
			VirtualPath = "~/emailviews";
			// BusConfigFileName = @".\ariane.config";
			SmtpClientFactory = new SmtpClientFactory();
			Logger = new DiagnosticsLogger();
			EmailerServiceFactory = new EmailerServiceFactory();
		}

		/// <summary>
		/// Resolver 
		/// </summary>
		public IDependencyResolver DependencyResolver { get; set; }
		/// <summary>
		/// Logger
		/// </summary>
		public ILogger Logger { get; set; }

		public IEmailerServiceFactory EmailerServiceFactory { get; set; }

		public string EmailBodyRequestedQueueName { get; set; }

		public string SendEmailQueueName { get; set; }

		public string SentMessageQueueName { get; set; }

		public string SentFailQueueName { get; set; }

		public string ReceiveMessageQueueName { get; set; }

		public string FullUrl { get; set; }

		public string SenderEmail { get; set; }

		public string SenderName { get; set; }

		public string SenderCode { get; set; }

		public string SenderJobTitle { get; set; }

		public string VirtualPath { get; set; }

		[Obsolete("using specific configuration for ariane", true)]
		public string BusConfigFileName { get; set; }

		public string ApiToken { get; set; }

		public string PhysicalPath { get; set; }

		public string DkimPrivateKey { get; set; }

		public string DkimDomain { get; set; }

		public string DkimSelector { get; set; }

		public string ImapHostName { get; set; }

		public int ImapPort { get; set; }

		public string ImapUserName { get; set; }

		public string ImapPassword { get; set; }

		public string ImapAuthMethod { get; set; }

		public bool ImapUseSSL { get; set; }

		public ISmtpClientFactory SmtpClientFactory { get; set; }

		public string EmailMessageSenderType { get; set; }
	}
}
