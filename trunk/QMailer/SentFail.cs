using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer
{
	public class SentFail
	{
		public SentFail()
		{
			Recipients = new List<EmailAddress>();
			FailDate = DateTime.Now;
		}

		public string MessageId { get; set; }
		public string Subject { get; set; }
		public string Message { get; set; }
		public string Stack { get; set; }
		public IList<EmailAddress> Recipients { get; set; }
		public DateTime FailDate { get; set; }
	}
}
