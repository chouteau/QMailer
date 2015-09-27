using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace QMailer.SvcHost
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : System.Configuration.Install.Installer
	{
		string m_ServiceName = null;

		public ProjectInstaller()
		{
			InitializeComponent();
			m_ServiceName = Program.GetServiceName();
			SetServiceName(m_ServiceName);
		}

		private void SetServiceName(string serviceName)
		{
			QMailerServiceInstaller.DisplayName = serviceName;
			QMailerServiceInstaller.ServiceName = serviceName;
		}

		protected override void OnBeforeInstall(IDictionary savedState)
		{
			string serviceName = m_ServiceName;
			Console.WriteLine("ServiceName : {0}", serviceName);
			savedState["ServiceName"] = serviceName;
			SetServiceName(serviceName);
			Console.WriteLine(serviceName);
			base.OnBeforeInstall(savedState);
		}
	}
}
