using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.MailJet.Models
{
	public class MessageHistory 
	{
		public int Count { get; set; }
		public int Total { get; set; }
		public List<DataItem> Data { get; set; }
	}
}
