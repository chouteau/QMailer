using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace QMailer
{
	[DataContract]
	public class EmailConfig
	{
		public EmailConfig ()
		{
			Recipients = new List<EmailAddress>();
			Headers = new List<EmailMessageHeader>();
			Parameters = new List<EmailMessageParameter>();
			Attachments = new List<Attachment>();
			DoNotTrack = false;
		}

		[DataMember]
		public string MessageId { get; set; }
		[DataMember]
		public string EmailName { get; set; }
		[DataMember]
		public object Model { get; set; }
		[DataMember]
		public string AssemblyQualifiedTypeNameModel { get; set; }
		[DataMember]
		public IList<EmailAddress> Recipients { get; private set; }
		[DataMember]
		public DateTime? ScheduleDate { get; set; }
		[DataMember]
		public List<EmailMessageParameter> Parameters { get; set; }
		[DataMember]
		public List<EmailMessageHeader> Headers { get; set; }
		[DataMember]
		public List<Attachment> Attachments { get; set; }
		[DataMember]
		public Sender Sender { get; set; }
		[DataMember]
		public Sender SenderAlias { get; set; }
		[DataMember]
		public bool DoNotTrack { get; set; }
		[DataMember]
		public int? EntityId { get; set; }
		[DataMember]
		public string EntityName { get; set; }
		[DataMember]
		public string EmailBodyRequestedQueueName { get; set; }
		[DataMember]
		public string SendEmailQueueName { get; set; }
		[DataMember]
		public string SentMessageQueueName { get; set; }
		[DataMember]
		public string SentFailQueueName { get; set; }
	}
}
