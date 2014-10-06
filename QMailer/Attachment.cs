using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace QMailer
{
	[DataContract]
	public class Attachment
	{
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Content { get; set; }
		[DataMember]
		public string ContentType { get; set; }
	}
}
