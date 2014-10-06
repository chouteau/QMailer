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
			QMailer.EmailerService.Current.Stop();
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
			var emailConfig = EmailerService.Current.CreateEmailConfig(messageId);
			emailConfig.SetView("test")
				.AddRecipient(new EmailAddress() { Address = "test@test.com" })
				.AddParameter("param1", "value1")
				.AddModel(model);

			EmailerService.Current.SendAsync(emailConfig);
		}
	}
}
