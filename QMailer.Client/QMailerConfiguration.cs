using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QMailer.Client
{
	public class QMailerConfiguration
	{
		public QMailerConfiguration()
		{
			UserAgent = "QMAilerClient35";
		}

		public string ApiUrl { get; set; }
		public string ApiKey { get; set; }
		public string UserAgent { get; set; }
	}
}
