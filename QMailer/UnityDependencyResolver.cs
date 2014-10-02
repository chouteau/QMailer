using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;

namespace QMailer
{
	public class UnityDependencyResolver : QMailer.IDependencyResolver
		, Ariane.IDependencyResolver 
	{
		private Microsoft.Practices.Unity.IUnityContainer m_Container;
		private bool m_IsContainerConfigured = false;

		public UnityDependencyResolver(Microsoft.Practices.Unity.IUnityContainer container)
		{
			m_Container = container;
			ConfigureDefaultServices();
		}

		#region IDependencyResolver Members

		public object Container
		{
			get
			{
				return m_Container;
			}
		}

		public Microsoft.Practices.Unity.IUnityContainer GetConfiguredContainer()
		{
			return m_Container;
		}

		public IEnumerable<object> GetAllServices()
		{
			return m_Container.Registrations;
		}

		public object GetService(Type serviceType)
		{
			return m_Container.Resolve(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return m_Container.ResolveAll(serviceType);
		}

		#endregion

		private void ConfigureDefaultServices()
		{
			if (m_IsContainerConfigured)
			{
				return;
			}
			var registered = m_Container.IsRegistered<Ariane.IServiceBus>();
			if (!registered)
			{
				Ariane.GlobalConfiguration.Configuration.DependencyResolver = new ArianeDependencyResolver(m_Container);
				m_Container.RegisterType<Ariane.IServiceBus, Ariane.BusManager>(new ContainerControlledLifetimeManager());
			}
			var bus = m_Container.Resolve<Ariane.IServiceBus>();

			var busConfigFileName = GlobalConfiguration.Configuration.BusConfigFileName;
			if (System.IO.File.Exists(busConfigFileName))
			{
				if (busConfigFileName.StartsWith(@".\"))
				{
					var path = System.IO.Path.GetDirectoryName(typeof(QMailer.EmailerService).Assembly.Location);
					busConfigFileName = System.IO.Path.Combine(path.TrimEnd('\\'), busConfigFileName.Replace(@".\", "").TrimStart('\\'));
				}
				bus.Register.AddFromConfig(busConfigFileName);
			}

			if (GlobalConfiguration.Configuration.IsStandAlone)
			{
				bus.Register.AddQueue(new Ariane.QueueSetting()
				{
					AutoStartReading = true,
					Name = GlobalConfiguration.Configuration.SendEmailQueueName,
					TypeMedium = typeof(Ariane.InMemoryMedium),
					TypeReader = typeof(QMailer.SendEmailMessageReader)
				});
			}

			m_Container.RegisterType<ILogger, DiagnosticsLogger>(new ContainerControlledLifetimeManager());

			m_IsContainerConfigured = true;
		}
	}
}
