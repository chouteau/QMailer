using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using QMailer;

namespace QMailer.SvcHost
{
	public partial class MainServiceHost
	{
		public MainServiceHost()
		{
		}

		public void Initialize()
		{
		}

		public void Start()
		{
			QMailerService.Start();
		}

		public void Stop()
		{
			QMailerService.Stop();
		}
	}
}
