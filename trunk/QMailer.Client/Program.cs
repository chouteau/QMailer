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
			var emailConfig = EmailerService.Current.CreateEmailConfig();
			emailConfig.SetView("testemail")
				.AddRecipient(new EmailAddress() { Address = "test@test.com" })
				.Sender("sender@test.com", "SenderName", "SenderJob");

			QMailer.EmailerService.Current.SendAsync(emailConfig);
			QMailer.EmailerService.Current.Start();

			Console.Read();

			QMailer.EmailerService.Current.Stop();
		}
	}
}
