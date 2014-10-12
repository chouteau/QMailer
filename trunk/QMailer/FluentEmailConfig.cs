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

		public static EmailConfig SetSenderAlias(this EmailConfig config, string email, string fullName, string jobTitle,string code, bool isEmployee, string bag)
		{
			config.SenderAlias = new Sender()
			{
				Email = email,
				DisplayName = fullName,
				JobTitle = jobTitle,
				Code = code,
				IsEmployee = isEmployee,
				Bag = bag
			};
			return config;
		}

		public static EmailConfig SetSender(this EmailConfig config, string email, string fullName, string jobTitle,string code, bool isEmployee)
		{
			config.Sender = new Sender()
				{
					Email = email,
					DisplayName = fullName,
					JobTitle = jobTitle,
					Code = code,
					IsEmployee = isEmployee
				};
			return config;
		}

		public static EmailConfig SetModel(this EmailConfig config, object model)
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
			config.AddAttachment(ms.GetBuffer(), name, contentType);
			return config;
		}

		public static EmailConfig AddAttachment(this EmailConfig config, byte[] content, string name, string contentType)
		{
			var base64 = System.Convert.ToBase64String(content);
			var attachment = new Attachment();
			attachment.Content = base64;
			attachment.ContentType = contentType;
			attachment.Name = name;

			config.Attachments.Add(attachment);
			return config;
		}

		public static EmailMessage AddAttachment(this EmailMessage message, string name, string contentType, string fileName)
		{
			var ms = new System.IO.MemoryStream();
			using (var stream = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
			{
				var buffer = new byte[1024];
				int pos = 0;
				while ((pos = stream.Read(buffer, 0, 1024)) > 0)
				{
					ms.Write(buffer, 0, pos);
				}
				stream.Close();
			}
			message.AddAttachment(ms.GetBuffer(), name, contentType);
			return message;
		}

		public static EmailMessage AddAttachment(this EmailMessage message, byte[] content, string name, string contentType)
		{
			var base64 = System.Convert.ToBase64String(content);
			var attachment = new Attachment();
			attachment.Content = base64;
			attachment.ContentType = contentType;
			attachment.Name = name;

			message.Attachments.Add(attachment);
			return message;
		}

		public static EmailConfig DoNotTrack(this EmailConfig config)
		{
			config.DoNotTrack = true;
			return config;
		}

		public static EmailConfig SetMetaData(this EmailConfig config, int entityId, string entityName)
		{
			config.EntityId = entityId;
			config.EmailName = entityName;
			return config;
		}
	}
}
