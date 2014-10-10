using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace QMailer.Web
{
	internal class EmailParser 
	{
		public EmailMessage CreateMailMessage(string emailViewOutput)
		{
			var message = new EmailMessage();
			InitializeMailMessage(message, emailViewOutput);
			var title = ParseTitle(emailViewOutput);
			message.Subject = title ?? "title";

			return message;
		}

		void InitializeMailMessage(EmailMessage message, string emailViewOutput)
		{
			var body = new StringBuilder();
			var headerStart = new Regex(@"^\s*([A-Za-z\-]+)\s*:\s*(.*)");
			var countOfBlankLine = 0;
			using (var reader = new StringReader(emailViewOutput))
			{
				// traitement des headers
				while (true)
				{
					var line = reader.ReadLine();
					if (string.IsNullOrWhiteSpace(line))
					{
						if (countOfBlankLine > 10)
						{
							break;
						}
						countOfBlankLine++;
						continue;
					}

					var match = headerStart.Match(line);
					if (!match.Success)
					{
						body.Append(line);
						continue;
					}

					var key = match.Groups[1].Value.ToLowerInvariant();
					var value = match.Groups[2].Value.TrimEnd();

					bool headerAdded = AssignEmailHeaderToMailMessage(key, value, message);
					if (!headerAdded)
					{
						body.Append(line);
					}
				}

                message.Body = body.ToString();
			}
		}

		bool AssignEmailHeaderToMailMessage(string key, string value, EmailMessage message)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return false;
			}
			string email = value.Trim().Trim(';');
			string displayName = null;
			var match = System.Text.RegularExpressions.Regex.Match(email, "<(?<email>.*[^>)])>");
			if (match.Success)
			{
				email = match.Groups["email"].Value;
				displayName = value.Replace("<" + email + ">", "").Trim();
			}

			bool headerAdded = true;
			switch (key)
			{
				case "to":
					var toEmailList = email.Split(';');
					foreach (var e in toEmailList)
					{
						var to = new EmailAddress() 
						{ 
							Address = e, 
							DisplayName = displayName, 
							SendingType = EmailSendingType.To 
						};
						if (!message.Recipients.Any(i => i.Address.Equals(to.Address, StringComparison.InvariantCultureIgnoreCase)))
						{
							message.Recipients.Add(to);
						}
					}
					break;
				case "from":
					//message.Sender = new Sender()
					//{
					//	Email = email,
					//	DisplayName = displayName
					//};
					break;
				case "subject":
					message.Subject = value;
					break;
				case "cc":
					var ccEmailList = email.Split(';');
					foreach (var e in ccEmailList)
					{
						var cc = new EmailAddress() 
						{ 
							Address = e, 
							DisplayName = displayName, 
							SendingType = EmailSendingType.CC 
						};
						message.Recipients.Add(cc);
					}
					break;
				case "bcc":
	                var emailList = email.Split(';');
					foreach (var e in emailList)
					{
						var bcc = new EmailAddress() 
						{ 
							Address = e, 
							SendingType = EmailSendingType.BCC 
						};
						message.Recipients.Add(bcc);
					}
					break;
				case "replyto":
					var replyto = new EmailAddress() 
					{ 
						Address = email, 
						DisplayName = displayName, 
						SendingType = EmailSendingType.ReplyTo 
					};
					message.Recipients.Add(replyto);
					break;
				//case "content-type":
				//    var charsetMatch = Regex.Match(value, @"\bcharset\s*=\s*(.*)$");
				//    if (charsetMatch.Success)
				//    {
				//        message.BodyEncoding = Encoding.GetEncoding(charsetMatch.Groups[1].Value);
				//    }
				//    break;
				default:
					// message.Headers[key] = value;
					headerAdded = false;
					break;
			}
			return headerAdded;
		}

		internal string ParseTitle(string emailViewOutput)
		{
			var match = System.Text.RegularExpressions.Regex.Match(emailViewOutput, @"<title>(?<t>[^<]*)</title>");
			if (match.Success)
			{
				return match.Groups["t"].Value;
			}
			return null;
		}
	}
}
