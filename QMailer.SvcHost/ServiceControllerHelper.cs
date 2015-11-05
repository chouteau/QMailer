using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.SvcHost
{
	public class ServiceControllerHelper
	{
		public static bool m_UninstallInProgress = false;

		public static void InstallService(string svcName, bool autostart = false)
		{
			var svc = GetWindowsService(svcName);

			if (svc == null)
			{
				ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
				System.Threading.Thread.Sleep(2 * 1000);
				svc = GetWindowsService(svcName);
				if (svc != null)
				{
					System.Diagnostics.Trace.WriteLine(string.Format("{0} installed with success", svcName));
				}
			}
			else
			{
				System.Diagnostics.Trace.WriteLine(string.Format("{0} already installed", svcName));
			}

			if (svc.Status == ServiceControllerStatus.Stopped && autostart)
			{
				System.Diagnostics.Trace.WriteLine(string.Format("Try to start {0}", svcName));
				try
				{
					svc.Start();
					svc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 15));
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(ex.Message);
				}
			}
		}

		public static void UninstallService(string svcName)
		{
			if (m_UninstallInProgress)
			{
				return;
			}
			m_UninstallInProgress = true;
			var svc = GetWindowsService(svcName);

			if (svc == null)
			{
				System.Diagnostics.Trace.WriteLine(string.Format("{0} is not installed", svcName));
				return;
			}

			System.Diagnostics.Trace.WriteLine(string.Format("uninstall {0}", svcName));

			bool stopped = svc.Status == ServiceControllerStatus.Stopped;
			if (!stopped)
			{
				System.Diagnostics.Trace.WriteLine(string.Format("try to stop {0}", svcName));
				try
				{
					svc.Stop();
					svc.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 15));
					stopped = true;
				}
				catch(Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(ex.Message);
				}
            }

			if (stopped)
			{
				System.Diagnostics.Trace.WriteLine(string.Format("{0} stopped", svcName));
				try
				{
					ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(ex.ToString());
				}
				System.Diagnostics.Trace.WriteLine(string.Format("{0} uninstalled", svcName));
			}
			else
			{
				System.Diagnostics.Trace.WriteLine("cant stop this service, try to stop manualy before uninstall");
			}
		}

		public static ServiceController GetWindowsService(string serviceName)
		{
			var services = ServiceController.GetServices();
			foreach (var item in services)
			{
				if (item.DisplayName.Equals(serviceName, StringComparison.InvariantCultureIgnoreCase))
				{
					return item;
				}
			}
			return null;
		}

	}
}
