using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;

namespace QMailer
{
	internal class ArianeDependencyResolver : Ariane.IDependencyResolver
	{
		private Microsoft.Practices.Unity.IUnityContainer m_Container;

		public ArianeDependencyResolver(Microsoft.Practices.Unity.IUnityContainer container)
		{
			m_Container = container;
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
	}
}
