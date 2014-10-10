using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			var messageId = Guid.NewGuid().ToString();
			var emailConfig = QMailerService.Current.CreateEmailConfig(messageId);
			emailConfig.SetView("testemail")
				.AddRecipient(new EmailAddress() { Address = "test@test.com" })
				.Sender("sender@test.com", "SenderName", "SenderJob","CodeTest");

			QMailerService.Current.SendAsync(emailConfig);
			QMailerService.Current.Start();

			Console.Read();

			QMailerService.Current.Stop();
		}
	}
}
