using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer
{
	public interface ISmtpClientFactory
	{
		System.Net.Mail.SmtpClient Create(IEnumerable<EmailAddress> recipientList);
	}
}
