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
			Console.WriteLine("starting QMailerSvcHost");
			if (args != null && args.Length > 0 && args[0] == "/console")
			{
				Console.WriteLine("console mode detected");
				var listener = new System.Diagnostics.ConsoleTraceListener();

				System.Diagnostics.Debug.Listeners.Add(listener);
				System.Diagnostics.Debug.AutoFlush = true;

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
	}
}
