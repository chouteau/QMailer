using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QMailer
{
	[DataContract]
	public class SentMessage
	{
		public SentMessage()
		{
			Recipients = new List<EmailAddress>();
			SentDate = DateTime.Now;
		}

		[DataMember]
		public string MessageId { get; set; }
		[DataMember]
		public string Subject { get; set; }
		[DataMember]
		public string Body { get; set; }
		[DataMember]
		public List<EmailAddress> Recipients { get; set; }
		[DataMember]
		public DateTime SentDate { get; set; }
		[DataMember]
		public string SmtpInfo { get; set; }
		[DataMember]
		public Sender Sender { get; set; }
		[DataMember]
		public bool IsBodyHtml { get; set; }
		[DataMember]
		public int? EntityId { get; set; }
		[DataMember]
		public string EntityName { get; set; }
	}
}
