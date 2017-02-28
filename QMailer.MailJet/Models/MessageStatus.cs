using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.MailJet.Models
{
	public enum MessageStatus
	{
		Queued,
		Sent,
		Opened,
		Clicked,
		Bounced,
		Blocked,
		Spam,
		Unsub
	}
}
