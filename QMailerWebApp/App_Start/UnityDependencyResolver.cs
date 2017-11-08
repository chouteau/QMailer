using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;

namespace QMailer
{
	public class UnityDependencyResolver : QMailer.IDependencyResolver
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

		public object GetService(string serviceName, Type serviceType)
		{
			return m_Container.Resolve(serviceType, serviceName);
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
			var local = @".\";
			if (busConfigFileName.StartsWith(local))
			{
				var path = System.Environment.CurrentDirectory.TrimEnd('\\');
				busConfigFileName = busConfigFileName.Replace(local, "").TrimStart('\\');
				busConfigFileName = System.IO.Path.Combine(path, busConfigFileName);
			}
			if (System.IO.File.Exists(busConfigFileName))
			{
				bus.Register.AddFromConfig(busConfigFileName);
			}

			// m_Container.RegisterType<ImapClientService>(new ContainerControlledLifetimeManager());
			if (GlobalConfiguration.Configuration.EmailMessageSenderType == null)
			{
				m_Container.RegisterType<IEmailMessageSender, StandardEmailMessageSender>(new PerResolveLifetimeManager());
			}
			else
			{
				var type = Type.GetType(GlobalConfiguration.Configuration.EmailMessageSenderType);
				m_Container.RegisterType(typeof(IEmailMessageSender), type, new PerResolveLifetimeManager());
			}

			m_IsContainerConfigured = true;
		}
	}
}
