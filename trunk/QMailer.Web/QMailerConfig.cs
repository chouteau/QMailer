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
		public static void Configure(Microsoft.Practices.Unity.IUnityContainer container)
		{
			if (QMailer.GlobalConfiguration.Configuration.DependencyResolver == null)
			{
				QMailer.GlobalConfiguration.Configuration.DependencyResolver = new QMailer.UnityDependencyResolver(container);
			}
			container = QMailer.GlobalConfiguration.Configuration.DependencyResolver.GetConfiguredContainer();

			var emailEngines = new ViewEngineCollection() { new EmailerViewEngine() };
			var ic = new InjectionConstructor(emailEngines, 
							GlobalConfiguration.Configuration.FullUrl, 
							GlobalConfiguration.Configuration.VirtualPath);
			container.RegisterType<IEmailViewRenderer, EmailViewRenderer>("default", new ContainerControlledLifetimeManager(), ic);

			container.RegisterType<IEmailTemplateService, EmailTemplateService>(new ContainerControlledLifetimeManager());
			container.RegisterType<ICacheService, SimpleCacheService>(new ContainerControlledLifetimeManager());

			if (!container.IsRegistered<QMailer.Web.IModelResolver>())
			{
				container.RegisterType<IModelResolver, DefaultModelResolver>(new ContainerControlledLifetimeManager());
			}
		}
	}
}