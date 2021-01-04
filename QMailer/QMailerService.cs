using System;
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

        private static Lazy<List<Type>> m_InterceptorList
			= new Lazy<List<Type>>();

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

        public static void RegisterInterceptor(Type interceptorType)
        {
            m_InterceptorList.Value.Add(interceptorType);
        }

        public static List<Type> GetInterceptorTypeList()
        {
            return m_InterceptorList.Value;
        }

		public static void OnStop(Action stop)
		{
			StopList.Add(stop);
		}

		internal static IEmailerService GetInstance()
		{
			return m_LazyInstance.Value;
		}

		private static IEmailerService InitializeService()
		{
			if (GlobalConfiguration.Configuration.DependencyResolver == null)
			{
				return null;
			}
			var bus = GlobalConfiguration.Configuration.DependencyResolver.GetService<Ariane.IServiceBus>();
			var esf = GlobalConfiguration.Configuration.EmailerServiceFactory;

			var emailerService = esf.Create();
			emailerService.ReplaceBusService(bus);

			return emailerService;
		}
	}
}
