using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer
{
	public class EmailConfig
	{
		public EmailConfig ()
		{
			Recipients = new List<EmailAddress>();
		}

		public string EmailName { get; set; }
		public object Model { get; set; }
		public string AssemblyQualifiedTypeNameModel { get; set; }
		public IList<EmailAddress> Recipients { get; private set; }
		public DateTime? ScheduleDate { get; set; }
	}
}
