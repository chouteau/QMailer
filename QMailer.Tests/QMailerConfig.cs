using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.Practices.Unity;

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

			var emailEngines = new ViewEngineCollection() { new EmailerViewEngine() };
			var ic = new InjectionConstructor(emailEngines, 
							GlobalConfiguration.Configuration.FullUrl ?? "http://localhost", 
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