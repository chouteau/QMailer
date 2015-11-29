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
			this.Body = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(mailMessage.Body));
			this.IsBodyHtml = mailMessage.IsBodyHtml;
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
			foreach (var header in mailMessage.Headers.AllKeys)
			{
				Headers.Add(new EmailMessageHeader()
					{
						Name = header,
						Value = mailMessage.Headers[header]
					});
			}
			Attachments = new List<Attachment>();
			foreach (var attachment in mailMessage.Attachments)
			{
				string content = null;
				int contentLength = 0;
				using(var reader = new System.IO.StreamReader(attachment.ContentStream))
				{
					content = reader.ReadToEnd();
					content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(content));
					contentLength = content.Length;
				}

				if (contentLength < 1024)
				{
					Attachments.Add(new Attachment()
					{
						Content = content,
						ContentType = attachment.ContentType.Name,
						Name = attachment.Name
					});
				}
				else
				{
					// ?
				}
			}
		}

		[DataMember]
		public string Subject { get; set; }
		[DataMember]
		public string Body { get; set; }
		[DataMember]
		public bool IsBodyHtml { get; set; }
		[DataMember]
		public List<EmailAddress> Recipients { get; set; }
		[DataMember]
		public Sender Sender { get; set; }
		[DataMember]
		public Sender SenderAlias { get; set; }
		[DataMember]
		public List<EmailMessageHeader> Headers { get; set; }
		[DataMember]
		public DateTime? StartDate { get; set; }
		[DataMember]
		public string MessageId { get; set; }
		[DataMember]
		public List<Attachment> Attachments { get; set; }
		[DataMember]
		public bool DoNotTrack { get; set; }
		[DataMember]
		public int? EntityId { get; set; }
		[DataMember]
		public string EntityName { get; set; }
		[DataMember]
		public string ImapMessageId { get; set; }

		[DataMember]
		public string EmailBodyRequestedQueueName { get; set; }
		[DataMember]
		public string SendEmailQueueName { get; set; }
		[DataMember]
		public string SentMessageQueueName { get; set; }
		[DataMember]
		public string SentFailQueueName { get; set; }


		public static explicit operator System.Net.Mail.MailMessage(EmailMessage template)
		{
			var mailMessage = new System.Net.Mail.MailMessage();
			foreach (var email in template.Recipients)
			{
                if (email.Address == null)
                {
                    continue;
                }
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
						mailMessage.ReplyTo = new System.Net.Mail.MailAddress(email.Address, email.DisplayName);
						break;
                    case EmailSendingType.ReturnPath:
                        mailMessage.Headers.Add("Return-Path", email.Address);
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
				var buffer = System.Convert.FromBase64String(attachment.Content);
				var ms = new System.IO.MemoryStream(buffer);
				var att = new System.Net.Mail.Attachment(ms, attachment.Name, attachment.ContentType);
				mailMessage.Attachments.Add(att);
			}

			foreach (var header in template.Headers)
			{
				mailMessage.Headers.Add(header.Name, header.Value);
			}

			return mailMessage;
		}
	}
}
