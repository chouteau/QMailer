using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer
{
	class SmtpClientFactory : ISmtpClientFactory
	{
		public System.Net.Mail.SmtpClient Create(IEnumerable<EmailAddress> recipientList)
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

	}
}
