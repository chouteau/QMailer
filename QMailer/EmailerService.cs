using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using Ariane;

namespace QMailer
{
	class EmailerService : QMailer.IEmailerService
	{
		internal EmailerService()
		{
		}

		internal Ariane.IServiceBus Bus { get; set; }

		public EmailConfig CreateEmailConfig(string messageId = null)
		{
			var result = new EmailConfig();
			result.MessageId = messageId ?? Guid.NewGuid().ToString();
			return result;
		}

		public void SendAsync(EmailConfig emailConfig)
		{
			var queueName = emailConfig.EmailBodyRequestedQueueName ?? GlobalConfiguration.Configuration.EmailBodyRequestedQueueName;
			Bus.Send(GlobalConfiguration.Configuration.EmailBodyRequestedQueueName, emailConfig);
		}

		public void ReplaceBusService(Ariane.IServiceBus bus)
		{
			Bus = bus;
		}

		public void Stop()
		{
			Bus.StopReading();
		}

		public void Start()
		{
			Bus.StartReading();
			GlobalConfiguration.Configuration.Logger.Info("Service started");
		}
	}
}
