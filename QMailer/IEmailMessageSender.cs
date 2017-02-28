using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer
{
	public interface IEmailMessageSender
	{
		void Send(EmailMessage message);
	}
}
