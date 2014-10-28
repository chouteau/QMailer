using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer
{
	public static class QMailerService
	{
		private static Lazy<IEmailerService> m_LazyInstance = new Lazy<IEmailerService>(InitializeService, true);

		public static IEmailerService Current
		{
			get
			{
				return m_LazyInstance.Value;
			}
		}

		private static EmailerService InitializeService()
		{
			if (GlobalConfiguration.Configuration.DependencyResolver == null)
			{
				var container = new Microsoft.Practices.Unity.UnityContainer();
				GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
			}
			var bus = GlobalConfiguration.Configuration.DependencyResolver.GetService<Ariane.IServiceBus>();

			var emailerService = new EmailerService();
			emailerService.Bus = bus;

			return emailerService;
		}
	}
}
