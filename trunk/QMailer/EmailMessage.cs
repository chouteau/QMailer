using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QMailer
{
	[DataContract(IsReference=true)]
	public class EmailMessage
	{
		public EmailMessage()
			: this(new System.Net.Mail.MailMessage())
		{
		}

		public EmailMessage(System.Net.Mail.MailMessage mailMessage)
		{
			this.Sender = new Sender();
			this.Subject = mailMessage.Subject;
			this.Body = mailMessage.Body;
			this.Recipients = new List<EmailAddress>();
			if (mailMessage.To != null)
			{
				foreach (var to in mailMessage.To)
				{
					Recipients.Add(new EmailAddress()
					{
						Address = to.Address,
						DisplayName = to.DisplayName,
						SendingType = EmailSendingType.To,
					});
				}
			}
			if (mailMessage.CC != null)
			{
				foreach (var to in mailMessage.CC)
				{
					Recipients.Add(new EmailAddress()
					{
						Address = to.Address,
						DisplayName = to.DisplayName,
						SendingType = EmailSendingType.CC,
					});
				}
			}
			if (mailMessage.Bcc != null)
			{
				foreach (var to in mailMessage.Bcc)
				{
					Recipients.Add(new EmailAddress()
					{
						Address = to.Address,
						DisplayName = to.DisplayName,
						SendingType = EmailSendingType.BCC,
					});
				}
			}

			if (mailMessage.From != null)
			{
				Sender = new Sender()
				{
					Email = mailMessage.From.Address,
					DisplayName = mailMessage.From.DisplayName,
				};

			}

			Headers = new List<EmailMessageHeader>();
			Attachments = new List<Attachment>();
		}

		[DataMember]
		public string Subject { get; set; }
		[DataMember]
		public string Body { get; set; }
		[DataMember]
		public List<EmailAddress> Recipients { get; set; }
		[DataMember]
		public Sender Sender { get; set; }
		[DataMember]
		public List<EmailMessageHeader> Headers { get; set; }
		[DataMember]
		public DateTime? StartDate { get; set; }
		[DataMember]
		public string MessageId { get; set; }
		[DataMember]
		public List<Attachment> Attachments { get; set; }

		public static explicit operator System.Net.Mail.MailMessage(EmailMessage template)
		{
			var mailMessage = new System.Net.Mail.MailMessage();
			foreach (var email in template.Recipients)
			{
				switch (email.SendingType)
				{
					case EmailSendingType.To:
						mailMessage.To.Add(new System.Net.Mail.MailAddress(email.Address, email.DisplayName));		 
						break;
					case EmailSendingType.CC:
						mailMessage.CC.Add(new System.Net.Mail.MailAddress(email.Address, email.DisplayName));		 
						break;
					case EmailSendingType.BCC:
						mailMessage.Bcc.Add(new System.Net.Mail.MailAddress(email.Address, email.DisplayName));		 
						break;
					case EmailSendingType.ReplyTo:
						// mailMessage.ReplyToList.Add(new System.Net.Mail.MailAddress(email.Address, email.DisplayName));		 
						break;
					default:
						break;
				}
			}

			try
			{
				mailMessage.Subject = template.Subject ?? "(sans sujet)";
				mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
			}
			catch
			{
				mailMessage.Subject = "(sans sujet)";
				template.Body = "subject :" + template.Subject + Environment.NewLine + template.Body;
			}
			mailMessage.Body = template.Body;
			mailMessage.IsBodyHtml = true;
			mailMessage.From = new System.Net.Mail.MailAddress(template.Sender.Email, template.Sender.DisplayName);

			foreach (var attachment in template.Attachments)
			{
				using(var ms = new System.IO.MemoryStream(System.Convert.FromBase64String(attachment.Content)))
				{
					var att = new System.Net.Mail.Attachment(ms, attachment.Name, attachment.ContentType);
					mailMessage.Attachments.Add(att);
				}
			}

			foreach (var header in template.Headers)
			{
				mailMessage.Headers.Add(header.Name, header.Value);
			}

			return mailMessage;
		}
	}
}
