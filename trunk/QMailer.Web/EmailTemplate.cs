using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QMailer.Web
{
	[DataContract]
    public class EmailTemplate
    {
		[IgnoreDataMember]
		public string FullName { get; set; }

		[DataMember]
		public string ViewName { get; set; }

		[DataMember]
		public string ShortName { get; set; }

		[DataMember]
		public String Content { get; set; }

		[DataMember]
		public DateTime CreationDate{get; set;}

		[DataMember]
		public DateTime LastUpdate { get; set; }

		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public bool IsCustom { get; set; }

		[DataMember]
		public string ModelName { get; set; }

		[DataMember]
		public string ModelDescription { get; set; }
    }
}
