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
		public Sender()
		{
			IsEmployee = false;
		}
		[DataMember]
		public string Email { get; set; }
		[DataMember]
		public string DisplayName { get; set; }
		[DataMember]
		public string JobTitle { get; set; }
		[DataMember]
		public string Code { get; set; }
		[DataMember]
		public bool IsEmployee { get; set; }
		[DataMember]
		public string Bag { get; set; }
	}
}
