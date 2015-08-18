using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.Tests
{
	class MockSmtpClientFactory : QMailer.ISmtpClientFactory
	{
		public SmtpClient Create(IEnumerable<EmailAddress> recipientList)
		{
			CreateCount++;
			var client = new SmtpClient();
			client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
			client.PickupDirectoryLocation = @".\emails";
			return client;
		}

		public int CreateCount { get; private set; }
	}
}
