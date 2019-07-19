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
        }

        [TestCleanup]
        public void TearDown()
        {
            QMailer.QMailerService.Stop();
        }


        [TestMethod]
        public void Send_Email()
        {
            var client = new QMailer.SendinBlue.SendInBlueClient("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");
            QMailer.SendinBlue.Models.CreateSmtpEmail result = null;

            var message = new EmailMessage();
            message.Body = "<html><body>Hello world</body></html>";
            message.Subject = "Test SendInBlue";
            message.IsBodyHtml = true;
            message.MessageId = Guid.NewGuid().ToString();
            message.Recipients = new List<EmailAddress>();
            message.Recipients.Add(new EmailAddress()
            {
                Address = "pascal.heilly@gmail.com",
                DisplayName = "Pascal Heilly",
                RecipientId = Guid.NewGuid().ToString(),
                SendingType = EmailSendingType.To
            });
            message.Sender = new Sender() {
                DisplayName =" Pascal",
                Email = "pascal@societe-crea.com"
            };

            result = client.SendMessage(message);

            Assert.IsNotNull(result);


            var statusCheck = client.GetMessageHistory(result.MessageId);

            Assert.IsNotNull(statusCheck);
            Assert.IsNotNull(statusCheck.Events);

            var lastStatus = statusCheck.Events.Last();
            Assert.IsNotNull(lastStatus);
        }


        [TestMethod]
        public void Send_Text_Email()
        {
            var client = new QMailer.SendinBlue.SendInBlueClient("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");
            QMailer.SendinBlue.Models.CreateSmtpEmail result = null;

            var message = new EmailMessage();
            message.Body = "Hello world";
            message.IsBodyHtml = false;
            message.Subject = "Test SendInBlue";
            message.MessageId = Guid.NewGuid().ToString();
            message.Recipients = new List<EmailAddress>();
            message.Recipients.Add(new EmailAddress()
            {
                Address = "pascal.heilly@gmail.com",
                DisplayName = "Pascal Heilly",
                RecipientId = Guid.NewGuid().ToString(),
                SendingType = EmailSendingType.To
            });
            message.Sender = new Sender()
            {
                DisplayName = " Pascal",
                Email = "pascal@societe-crea.com"
            };

            result = client.SendMessage(message);

            Assert.IsNotNull(result);


            var statusCheck = client.GetMessageHistory(result.MessageId);

            Assert.IsNotNull(statusCheck);
            Assert.IsNotNull(statusCheck.Events);

            var lastStatus = statusCheck.Events.Last();
            Assert.IsNotNull(lastStatus);
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
