using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;

using Ariane;
using System.Web.Mvc;

namespace QMailer.Tests
{
	public static class TestHelpers
	{
		public static void Initialize()
		{
			var container = new Microsoft.Practices.Unity.UnityContainer();
			GlobalConfiguration.Configuration.DependencyResolver = new QMailer.UnityDependencyResolver(container);

			GlobalConfiguration.Configuration.FullUrl = "http://localhost";
			GlobalConfiguration.Configuration.SenderEmail = "test@test.com";
			GlobalConfiguration.Configuration.SenderName = "TestEmail";

			var existingBus = container.Resolve<Ariane.IServiceBus>();
			container.RegisterType<Ariane.IServiceBus, Ariane.SyncBusManager>(new ContainerControlledLifetimeManager(), new InjectionConstructor(existingBus));

			var bus = container.Resolve<Ariane.IServiceBus>();
			bus.Register.AddQueue(new QueueSetting()
				{
					AutoStartReading = true,
					Name = GlobalConfiguration.Configuration.EmailBodyRequestedQueueName,
					TypeMedium = typeof(Ariane.InMemoryMedium),
					TypeReader = typeof(QMailer.Web.EmailBodyRequestedMessageReader)
				});
			bus.Register.AddQueue(new QueueSetting()
				{
					AutoStartReading = true,
					Name = GlobalConfiguration.Configuration.SendEmailQueueName,
					TypeMedium = typeof(Ariane.InMemoryMedium),
					TypeReader = typeof(QMailer.SendEmailMessageReader)
				});

			QMailerService.Current.ReplaceBusService(bus);
			QMailer.Web.QMailerConfig.Configure(container);

			var viewEngines = new Moq.Mock<ViewEngineCollection>();
			var fakeView = new FakeView();
			viewEngines.Setup(i => i.FindView(Moq.It.IsAny<ControllerContext>(), "test", null))
				.Returns(new ViewEngineResult(fakeView, Moq.Mock.Of<IViewEngine>()));

			var fakeRenderer = new QMailer.Web.EmailViewRenderer(viewEngines.Object, "http://localhost", "~/emailviews");
			container.RegisterInstance<QMailer.Web.IEmailViewRenderer>(fakeRenderer, new ContainerControlledLifetimeManager());

			container.RegisterType<QMailer.ILogger, QMailer.DiagnosticsLogger>(new ContainerControlledLifetimeManager());
		}
	}
}
