using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QMailer.Tests
{
	[TestClass]
	public class MessageInterceptorTests
	{
		[TestInitialize]
		public void Initialize()
		{
			TestHelpers.Initialize();
			Bus = GlobalConfiguration.Configuration.DependencyResolver.GetService<Ariane.IServiceBus>();
		}

		protected Ariane.IServiceBus Bus { get; private set; }

		[TestCleanup]
		public void TearDown()
		{
			QMailer.QMailerService.Stop();
		}

		[TestMethod]
        [Ignore]
        
        public void Send_Email_WithInterceptor()
        {
			var sender = new QMailer.SendinBlue.SendInBlueSender(Bus);

			var emailSender = System.Configuration.ConfigurationManager.AppSettings["SIBSenderEmail"];
			var emailRecipient = System.Configuration.ConfigurationManager.AppSettings["SIBRecipientEmail"];


            QMailerService.RegisterInterceptor(typeof(MockInterceptor));

            var message = new EmailMessage();
            message.Body = "Hello world";
            message.IsBodyHtml = false;
            message.Subject = "Test SendInBlue";
            message.MessageId = Guid.NewGuid().ToString();
            message.Recipients = new List<EmailAddress>();
            message.Recipients.Add(new EmailAddress()
            {
				Address = emailRecipient,
				DisplayName = $"-- {emailRecipient} --",
				RecipientId = Guid.NewGuid().ToString(),
                SendingType = EmailSendingType.To
            });
            message.Sender = new Sender()
            {
				DisplayName = $"-- {emailSender} --",
				Email = emailSender
			};

            sender.Send(message);

            DateTime retryMaxTime = DateTime.Now.Add(new TimeSpan(0, 1, 0));

            while(message.Body != "executed" && DateTime.Now<retryMaxTime)
            {
                Thread.Sleep(1000);
            }
            Assert.IsTrue(message.Body == "executed");
        }

        private string GetFirstGoodErrorMessage(Exception ex)
        {
            var x = ex;
            var loop = 0;
            while (true)
            {
                if (x.InnerException == null)
                {
                    break;
                }
                if (loop > 10)
                {
                    break;
                }
                x = x.InnerException;
                loop++;
            }
            return x.Message;
        }
    }
}
