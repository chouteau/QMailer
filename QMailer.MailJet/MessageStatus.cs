using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.MailJet
{
	public enum MessageStatus
	{
		Pending,
		Queued,
		Sent,
		Opened,
		Clicked,
		Bounce,
		Blocked,
		Spam,
		Unsub
	}
}
