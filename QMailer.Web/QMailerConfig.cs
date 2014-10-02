using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Practices.Unity;
using System.Web.Mvc;

namespace QMailer.Web
{
	public class QMailerConfig
	{
		public static void Configure(Microsoft.Practices.Unity.IUnityContainer container, bool isStandAlone)
		{
			GlobalConfiguration.Configuration.IsStandAlone = isStandAlone;
			if (QMailer.GlobalConfiguration.Configuration.DependencyResolver == null)
			{
				QMailer.GlobalConfiguration.Configuration.DependencyResolver = new QMailer.UnityDependencyResolver(container);
			}
			container = QMailer.GlobalConfiguration.Configuration.DependencyResolver.GetConfiguredContainer();

			var emailEngines = new ViewEngineCollection() { new EmailerViewEngine() };
			container.RegisterType<IEmailViewRenderer, EmailViewRenderer>(new ContainerControlledLifetimeManager(), new InjectionConstructor(emailEngines));

			container.RegisterType<EmailTemplateService>(new ContainerControlledLifetimeManager());

			if (GlobalConfiguration.Configuration.IsStandAlone)
			{
				var bus = container.Resolve<Ariane.IServiceBus>();
				bus.Register.AddQueue(new Ariane.QueueSetting()
				{
					AutoStartReading = true,
					Name = GlobalConfiguration.Configuration.EmailBodyRequestedQueueName,
					TypeMedium = typeof(Ariane.InMemoryMedium),
					TypeReader = typeof(QMailer.Web.EmailBodyRequestedMessageReader)
				});
			}
		}
	}
}