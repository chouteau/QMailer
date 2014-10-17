using System;

namespace QMailer
{
	public interface IEmailerService
	{
		EmailConfig CreateEmailConfig(string messageId);
		void ReplaceBusService(Ariane.IServiceBus bus);
		void SendAsync(EmailConfig emailConfig);
		void Start();
		void Stop();
	}
}
