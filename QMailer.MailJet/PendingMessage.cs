using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.MailJet
{
	public class PendingMessage
	{
		public string MessageId { get; set; }
		public string Subject { get; set; }
		public IList<EmailAddress> Recipients { get; set; }
		public long MailJetMessageId { get; set; }
		public string Email { get; set; }
		public DateTime SendDate { get; set; }
		public DateTime? LastCheckDate { get; set; }
		public int CheckCount { get; set; }
	}
}
