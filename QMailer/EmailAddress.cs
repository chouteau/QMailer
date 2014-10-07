using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QMailer
{
	[DataContract]
	public class EmailAddress
	{
		public EmailAddress(string address = null, string displayName = null, EmailSendingType sendingType = EmailSendingType.To)
		{
			SendingType = sendingType;
			Address = address;
			DisplayName = displayName;
		}

		[DataMember]
		public string RecipientId { get; set; }
		[DataMember]
		public string Address { get; set; }
		[DataMember]
		public string DisplayName { get; set; }
		[DataMember]
		public EmailSendingType SendingType { get; set; }
	}
}
