﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer
{
	public static class QMailerService
	{
		private static Lazy<IEmailerService> m_LazyInstance 
			= new Lazy<IEmailerService>(InitializeService, true);

		private static List<Action> StopList { get; set; } = new List<Action>();

		public static EmailConfig CreateEmailConfig(string messageId = null)
		{
			return m_LazyInstance.Value.CreateEmailConfig(messageId);
		}

		public static void ReplaceBusService(Ariane.IServiceBus bus)
		{
			m_LazyInstance.Value.ReplaceBusService(bus);
		}

		public static void SendAsync(EmailConfig emailConfig)
		{
			m_LazyInstance.Value.SendAsync(emailConfig);
		}

		public static void Start()
		{
			m_LazyInstance.Value.Start();
		}

		public static void Stop()
		{
			foreach (var item in StopList)
			{
				item.Invoke();
			}
			m_LazyInstance.Value.Stop();
		}

		public static void OnStop(Action stop)
		{
			StopList.Add(stop);
		}

		internal static IEmailerService GetInstance()
		{
			return m_LazyInstance.Value;
		}

		private static EmailerService InitializeService()
		{
			//if (GlobalConfiguration.Configuration.DependencyResolver == null)
			//{
			//	var container = new Microsoft.Practices.Unity.UnityContainer();
			//	GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
			//}

			if (GlobalConfiguration.Configuration.DependencyResolver == null)
			{
				return null;
			}
			var bus = GlobalConfiguration.Configuration.DependencyResolver.GetService<Ariane.IServiceBus>();

			var emailerService = new EmailerService();
			emailerService.Bus = bus;

			return emailerService;
		}
	}
}
