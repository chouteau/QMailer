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

	}
}
