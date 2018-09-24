using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.MailJet.Models
{
	public class SentEvent
	{
		public string Event { get; set; }
		public string Time { get; set; }
		public string MessageID { get; set; }
		public string Email { get; set; }
		public string Mj_contact_id { get; set; }
		public string Customcampaign { get; set; }
		public string Mj_message_id { get; set; }
		public string Smtp_reply { get; set; }
		public string CustomID { get; set; }
		public string Payload { get; set; }
	}
}
