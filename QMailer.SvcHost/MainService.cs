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
	public partial class MainService : ServiceBase
	{
		public MainService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			StartQMailer();
		}

		public static void StartQMailer()
		{
			Microsoft.Practices.Unity.IUnityContainer container = new Microsoft.Practices.Unity.UnityContainer();
			QMailer.GlobalConfiguration.Configuration.DependencyResolver 
				= new QMailer.UnityDependencyResolver(container);

			QMailerService.Start();
		}

		protected override void OnStop()
		{
			QMailerService.Stop();
		}
	}
}
