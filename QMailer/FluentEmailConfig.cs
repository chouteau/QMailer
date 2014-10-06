using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer
{
	public static class FluentEmailConfigExtensions
	{
		public static EmailConfig AddRecipient(this EmailConfig config, string email, string recipientName = null)
		{
			var address = new EmailAddress();
			address.Address = email;
			address.DisplayName = recipientName ?? email;
			address.SendingType = EmailSendingType.To;
			config.AddRecipient(address);
			return config;
		}

		public static EmailConfig AddRecipient(this EmailConfig config, EmailAddress address)
		{
			config.Recipients.Add(address);
			return config;
		}

		public static EmailConfig Sender(this EmailConfig config, string email, string fullName, string jobTitle)
		{
			config.Sender = new Sender()
				{
					Email = email,
					DisplayName = fullName,
					JobTitle = jobTitle
				};
			return config;
		}

		public static EmailConfig AddModel(this EmailConfig config, object model)
		{
			config.Model = model;
			config.AssemblyQualifiedTypeNameModel = model.GetType().AssemblyQualifiedName;
			return config;
		}

		public static EmailConfig SetView(this EmailConfig config, string viewName)
		{
			config.EmailName = viewName;
			return config;
		}

		public static EmailConfig AddParameter(this EmailConfig config, string name, string value)
		{
			config.Parameters.Add(new EmailMessageParameter() { Name = name, Value = value });
			return config;
		}

		public static EmailConfig AddHeader(this EmailConfig config, string name, string value)
		{
			config.Headers.Add(new EmailMessageHeader() { Name = name, Value = value });
			return config;
		}

		public static EmailConfig AddAttachment(this EmailConfig config, string name, string contentType, string fileName)
		{
			var ms = new System.IO.MemoryStream();
			using (var stream = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
			{
				var buffer = new byte[1024];
				int pos = 0;
				while((pos = stream.Read(buffer, 0, 1024)) > 0)
				{
					ms.Write(buffer, 0, pos);
				}
				stream.Close();
			}
			var base64 = System.Convert.ToBase64String(ms.GetBuffer());
			var attachment = new Attachment();
			attachment.Content = base64;
			attachment.ContentType = contentType;
			attachment.Name = name;

			config.Attachments.Add(attachment);
			return config;
		}
	}
}
