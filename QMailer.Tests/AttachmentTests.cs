using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.Tests
{
	[TestClass]
	public class AttachmentTests
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
		public void Add_Attachment()
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
				.SetSender("marc@test.com", "marc", "god", "code", true)
				.AddAttachment("filename.pdf", "application/pdf", @".\file.pdf")
				.SetModel(model);


			System.IO.File.WriteAllText(@".\base64.txt", emailConfig.Attachments[0].Content);

		}

	}
}
