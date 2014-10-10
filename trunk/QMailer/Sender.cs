using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QMailer
{
	[DataContract]
	public class Sender
	{
		[DataMember]
		public string Email { get; set; }
		[DataMember]
		public string DisplayName { get; set; }
		[DataMember]
		public string JobTitle { get; set; }
		[DataMember]
		public string Code { get; set; }
	}
}
