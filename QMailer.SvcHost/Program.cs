using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.SvcHost
{
	static class Program 
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			var serviceName = GetServiceName();

			Console.WriteLine("starting " + serviceName);
			if (System.Environment.UserInteractive)
			{
				Console.WriteLine("console mode detected");

				string parameter = string.Concat(args);
				switch (parameter)
				{
					case "/install":
						ServiceControllerHelper.InstallService(serviceName);
						break;
					case "/uninstall":
						ServiceControllerHelper.UninstallService(serviceName);
						break;
					default:
						try
						{
							MainService.StartQMailer();
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
							System.Diagnostics.EventLog.WriteEntry("Application", ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
							return;
						}

						Console.WriteLine("hoster started");

						System.Console.Read();

						QMailerService.Stop();
						break;
				}
			}
			else
			{

				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[] 
				{ 
					new MainService() 
				};
				ServiceBase.Run(ServicesToRun);
			}
		}

		public static string GetServiceName()
		{
			var exePath = System.IO.Path.Combine(Environment.CurrentDirectory, "QMailerSvcHost.exe");
			System.Diagnostics.Trace.WriteLine(exePath);
			var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(exePath);
			var result = "QMailerSvcHost";
			if (config.AppSettings.Settings["QMailerSvcHost.ServiceName"] != null)
			{
				result = config.AppSettings.Settings["QMailerSvcHost.ServiceName"].Value;
			}
			Console.WriteLine("ServiceName : {0}", result);
			return result;
		}
	}
}
