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

			var emailConfig = EmailerService.Current.CreateEmailConfig();
			emailConfig.SetView("test")
				.AddRecipient(new EmailAddress() { Address = "test@test.com" })
				.AddModel(model);

			EmailerService.Current.SendAsync(emailConfig);
		}
	}
}
