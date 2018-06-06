using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using QMailer;

namespace QMailer.Tests
{
	[TestClass]
	public class EmailerServiceTests
	{
		public EmailerServiceTests()
		{

		}

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
		public void Parse_Title()
		{
			var title = "test D&#39;X";
			title = System.Web.HttpUtility.HtmlDecode(title);

			Assert.AreEqual("test D'X", title);
		}

		[TestMethod]
		public void Send_Email()
		{
			var model = new TestModel()
			{
				FirstName = "TestFirstName",
				LastName = "TestLastName",
				CreationDate = DateTime.Now
			};

			var messageId = Guid.NewGuid().ToString();
			var emailConfig = QMailerService.CreateEmailConfig(messageId);
			emailConfig.SetView("test")
				.AddRecipient(new EmailAddress() { Address = "test@test.com" })
				.AddParameter("param1", "value1")
				.SetSender("marc@test.com","marc","god","code",true)
				.SetModel(model);

			QMailerService.SendAsync(emailConfig);
		}

		// [TestMethod]
		public void Send_Email_With_Custom_Smtp_Factory()
		{
			var model = new TestModel()
			{
				FirstName = "TestFirstName",
				LastName = "TestLastName",
				CreationDate = DateTime.Now
			};

			var smtpClient = new MockSmtpClientFactory();
			GlobalConfiguration.Configuration.SmtpClientFactory = smtpClient;

			var messageId = Guid.NewGuid().ToString();
			var emailConfig = QMailerService.CreateEmailConfig(messageId);
			emailConfig.SetView("test")
				.AddRecipient(new EmailAddress() { Address = "test@test.com" })
				.AddParameter("param1", "value1")
				.SetSender("marc@test.com", "marc", "god", "code", true)
				.SetModel(model);

			QMailerService.SendAsync(emailConfig);

			Assert.AreNotEqual(0, smtpClient.CreateCount);
		}
	}
}
