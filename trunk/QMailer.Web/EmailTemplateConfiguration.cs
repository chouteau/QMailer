using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.Web
{
	public class EmailTemplateConfiguration
	{
		public string SenderEmail { get; set;}
		public string SenderDisplayName { get; set;}
		public string SenderCode { get; set; }
		public string SenderJobTitle { get; set; }
		public string PhysicalPath { get; set; }
		public string FullUrl { get; set; }
		public string VirtualPath { get; set; }
		public string RendererName { get; set; }
	}
}
