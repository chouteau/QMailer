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
		internal EmailConfig ()
		{
			Recipients = new List<EmailAddress>();
			Headers = new List<EmailMessageHeader>();
			Parameters = new List<EmailMessageParameter>();
		}

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

	}
}
