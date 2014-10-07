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
			BusConfigFileName = @".\ariane.config";
		}

		/// <summary>
		/// Resolver 
		/// </summary>
		public IDependencyResolver DependencyResolver { get; set; }
		/// <summary>
		/// Logger
		/// </summary>
		public ILogger Logger { get; set; }

		public string EmailBodyRequestedQueueName { get; set; }

		public string SendEmailQueueName { get; set; }

		public string SentMessageQueueName { get; set; }

		public string SentFailQueueName { get; set; }

		public string FullUrl { get; set; }

		public string FromEmail { get; set; }

		public string FromName { get; set; }

		public string VirtualPath { get; set; }

		public string BusConfigFileName { get; set; }

		public string ApiToken { get; set; }

		public string PhysicalPath { get; set; }

		public string DkimPrivateKey { get; set; }

		public string DkimDomain { get; set; }

		public string DkimSelector { get; set; }

	}
}
