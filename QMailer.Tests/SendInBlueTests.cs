using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.Tests
{
    [TestClass]
    public class SendInBlueTests
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
        public void Send_Email()
        {
			var sender = new QMailer.SendinBlue.SendInBlueSender(Bus);

			var emailSender = System.Configuration.ConfigurationManager.AppSettings["SIBSenderEmail"];
			var emailRecipient = System.Configuration.ConfigurationManager.AppSettings["SIBRecipientEmail"];

			var message = new EmailMessage();
            message.Body = "<html><body>Hello world</body></html>";
            message.Subject = "Test SendInBlue";
            message.IsBodyHtml = true;
            message.MessageId = Guid.NewGuid().ToString();
            message.Recipients = new List<EmailAddress>();
            message.Recipients.Add(new EmailAddress()
            {
                Address = emailRecipient,
                DisplayName = $"-- {emailRecipient} --",
                RecipientId = Guid.NewGuid().ToString(),
                SendingType = EmailSendingType.To
            });
            message.Sender = new Sender() {
                DisplayName = $"-- {emailSender} --",
				Email = emailSender
			};

            sender.Send(message);

			// SendinBlue.StatusChecker.Current.CheckPendingMessage()

            //Assert.IsNotNull(result);


            //var statusCheck = client.GetMessageHistory(result.MessageId);

            //Assert.IsNotNull(statusCheck);
            //Assert.IsNotNull(statusCheck.Events);

            //var lastStatus = statusCheck.Events.Last();
            //Assert.IsNotNull(lastStatus);
        }


        [TestMethod]
        public void Send_Text_Email()
        {
			var sender = new QMailer.SendinBlue.SendInBlueSender(Bus);

			var emailSender = System.Configuration.ConfigurationManager.AppSettings["SIBSenderEmail"];
			var emailRecipient = System.Configuration.ConfigurationManager.AppSettings["SIBRecipientEmail"];

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

            //Assert.IsNotNull(result);


            //var statusCheck = client.GetMessageHistory(result.MessageId);

            //Assert.IsNotNull(statusCheck);
            //Assert.IsNotNull(statusCheck.Events);

            //var lastStatus = statusCheck.Events.Last();
            //Assert.IsNotNull(lastStatus);
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
