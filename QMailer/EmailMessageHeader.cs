using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QMailer
{
	[DataContract]
	public class EmailMessageHeader
	{
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Value { get; set; }
	}
}
