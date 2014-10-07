using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.Web
{
	[DataContract]
	public class TemplateInfo
	{
		[DataMember]
		public string Id { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string ViewName { get; set; }
		[DataMember]
		public string ShortName { get; set; }
		[DataMember]
		public string Content { get; set; }
		[DataMember]
		public DateTime CreationDate { get; set; }
		[DataMember]
		public DateTime LastUpdate { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public string ModelName { get; set; }
		[DataMember]
		public bool IsCustom { get; set; }
	}
}

