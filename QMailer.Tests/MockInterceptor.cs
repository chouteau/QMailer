using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.Tests
{
	public class MockInterceptor : IMessageInterceptor
	{
		public EmailMessage ExecuteInterceptor(EmailMessage message)
		{
			message.Body = "excuted";
			return message;
		}
	}
}
