using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace QMailer
{
	[DataContract]
	public enum EmailSendingType
	{
		[EnumMember]
		To,
		[EnumMember]
		CC,
		[EnumMember]
		BCC,
		[EnumMember]
		ReplyTo,
	}
}
