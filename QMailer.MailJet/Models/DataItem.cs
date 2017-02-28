using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.MailJet.Models
{
	public class DataItem
	{
		public string Comment { get; set; }
		public long EventAt { get; set; }
		public MessageStatus EventType { get; set; }
		public string State { get; set; }
		public string UserAgent { get; set; }
	}
}
