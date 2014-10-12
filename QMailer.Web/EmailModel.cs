using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.Web
{
	public class EmailModel
	{
		public object Model { get; set; }
		public int? EntityId { get; set; }
		public string EntityName { get; set; }
	}
}
