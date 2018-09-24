using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.MailJet
{
	public class MailJetClient
	{
		public MailJetClient(string publicKey, string privateKey)
		{
			var authentication = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{publicKey}:{privateKey}"));
			Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authentication);
		}

		public System.Net.Http.Headers.AuthenticationHeaderValue Authorization { get; set; }

		public Models.SentMessageData SendMessage(EmailMessage emailMessage)
		{
			var content = new JObject();

			if (String.IsNullOrWhiteSpace(emailMessage.Sender.DisplayName))
			{
				content.Add("FromEmail", emailMessage.Sender.Email);
			}
			else
			{
				content.Add("FromEmail", emailMessage.Sender.Email);
				content.Add("FromName", emailMessage.Sender.DisplayName);
			}

			content.Add("Subject", emailMessage.Subject);

			var headers = new JObject();

			var destDic = new Dictionary<string, List<string>>();
			foreach (var address in emailMessage.Recipients)
			{
				string addressName = null;
				if (string.IsNullOrWhiteSpace(address.DisplayName))
				{
					addressName = address.Address;
				}
				else
				{
					addressName = $"{address.DisplayName} <{address.Address}>";
				}

				if (address.SendingType == EmailSendingType.To)
				{
					if (!destDic.ContainsKey("To"))
					{
						destDic.Add("To", new List<string>());
					}
					destDic["To"].Add(addressName);
				}
				else if (address.SendingType == EmailSendingType.BCC)
				{
					if (!destDic.ContainsKey("BCC"))
					{
						destDic.Add("BCC", new List<string>());
					}
					destDic["BCC"].Add(addressName);
				}
				else if (address.SendingType == EmailSendingType.CC)
				{
					if (!destDic.ContainsKey("CC"))
					{
						destDic.Add("CC", new List<string>());
					}
					destDic["CC"].Add(addressName);
				}
				else if (address.SendingType == EmailSendingType.ReplyTo)
				{
					headers.Add("Reply-To", addressName);
				}
				else if (address.SendingType == EmailSendingType.ReturnPath)
				{
					headers.Add("Return-Path", addressName);
				}
			}
			foreach (var key in destDic.Keys)
			{
				content.Add(key, string.Join(",", destDic[key]));
			}

			content.Add("Html-part", emailMessage.Body);

			if (emailMessage.Headers.Count > 0)
			{
				foreach (var header in emailMessage.Headers)
				{
					headers.Add(header.Name, header.Value);
				}
			}

			if (!string.IsNullOrWhiteSpace(emailMessage.MessageId))
			{
				headers.Add("Mj-CustomID", emailMessage.MessageId);
			}

			if (headers.Count > 0)
			{
				content.Add("Headers", headers);
			}

			if (emailMessage.Attachments.Any())
			{
				JArray attachments = new JArray();
				foreach (var item in emailMessage.Attachments)
				{
					var buffer = System.Convert.FromBase64String(item.Content);
					var ms = new System.IO.MemoryStream(buffer);

					var attachment = new JObject();
					attachment.Add("Content-type", new JValue(item.ContentType));
					attachment.Add("Filename", new JValue(item.Name));
					string file = Convert.ToBase64String(ms.ToArray());
					attachment.Add("content", new JValue(file));
					attachments.Add(attachment);
				}
				content.Add("Attachments", attachments);
			}

			var result = Execute<Models.SentMessageData>(client =>
			{
				var contentString = content.ToString();
				var httpContent = new StringContent(contentString, Encoding.UTF8, "application/json");
				var response = client.PostAsync("send", httpContent).Result;
				return response;
			});

			return result;
		}

		public Models.MessageHistory GetMessageHistory(long messageId)
		{
			var result = Execute<Models.MessageHistory>(client =>
			{
				return client.GetAsync($"REST/messagehistory/{messageId}").Result;
			});

			return result;
		}

		private T Execute<T>(Func<HttpClient, HttpResponseMessage> predicate)
		{
			var handler = new HttpClientHandler()
			{
				AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
				AllowAutoRedirect = false,
				UseCookies = false,
			};

			T result = default(T);
			using (var client = new HttpClient(handler))
			{
				client.BaseAddress = new Uri("https://api.mailjet.com/v3/");
				client.DefaultRequestHeaders.Add("UserAgent", "QMailer Client");
				client.DefaultRequestHeaders.Authorization = Authorization;
				var response = predicate.Invoke(client);
				response.EnsureSuccessStatusCode();
				var content = response.Content.ReadAsStringAsync().Result;
				result = response.Content.ReadAsAsync<T>().Result;
			}

			return result;
		}
	}
}
