using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QMailer.SendinBlue
{
    public class SendInBlueClient
    {
        private string m_ApiKey;
        public SendInBlueClient(string apiKey)
        {
            this.m_ApiKey = apiKey;
        }

        public Models.SendSmtpEmail CreateSendinBlueMessage(EmailMessage emailMessage)
        {
            var content = new Models.SendSmtpEmail();
            var headers = new JObject();
            content.Sender = new Models.SendSmtpEmailRecipient()
            {
                Email = emailMessage.Sender.Email,
                Name = emailMessage.Sender.DisplayName
            };

            if (emailMessage.Attachments.Any())
            {
                content.Attachment = new List<Models.SendSmtpEmailAttachment>();
                foreach (var item in emailMessage.Attachments)
                {
                    var buffer = System.Convert.FromBase64String(item.Content);
                    var ms = new System.IO.MemoryStream(buffer);

                    var attachment = new Models.SendSmtpEmailAttachment(null, ms.ToArray(), item.Name);
                    content.Attachment.Add(attachment);
                }
            }

            foreach (var address in emailMessage.Recipients)
            {
                var recipient = new Models.SendSmtpEmailRecipient()
                {
                    Email = address.Address,
                    Name = address.DisplayName
                };


                if (address.SendingType == EmailSendingType.To)
                {
                    if (content.To == null)
                    {
                        content.To = new List<Models.SendSmtpEmailRecipient>();
                    }

                    content.To.Add(recipient);
                }
                else if (address.SendingType == EmailSendingType.BCC)
                {
                    if (content.Bcc == null)
                    {
                        content.Bcc = new List<Models.SendSmtpEmailRecipient>();
                    }
                    content.Bcc.Add(recipient);
                }
                else if (address.SendingType == EmailSendingType.CC)
                {
                    if (content.Cc == null)
                    {
                        content.Cc = new List<Models.SendSmtpEmailRecipient>();
                    }
                    content.Cc.Add(recipient);
                }
                else if (address.SendingType == EmailSendingType.ReplyTo)
                {
                    content.ReplyTo = recipient;
                }
                else if (address.SendingType == EmailSendingType.ReturnPath)
                {
                    headers.Add("Return-Path", address.DisplayName);
                }
            }

            content.HtmlContent = emailMessage.Body;
            /*
            if (emailMessage.IsBodyHtml)
            {
                content.HtmlContent = emailMessage.Body;
            }
            else
            {
                content.TextContent = emailMessage.Body;
            } */
            content.Subject = emailMessage.Subject;

            if (emailMessage.Headers.Count > 0)
            {
                foreach (var header in emailMessage.Headers)
                {
                    headers.Add(header.Name, header.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(emailMessage.MessageId))
            {
                headers.Add("SIB-CustomID", emailMessage.MessageId);
            }

            if (headers.Count > 0)
            {
                content.Headers = headers;
            }

            return content;
        }


        public Models.CreateSmtpEmail SendMessage(Models.SendSmtpEmail senfinBlueMessage)
        {
            var result = Execute<Models.CreateSmtpEmail>(client =>
            {
                // var contentString = JsonConvert.SerializeObject(content);
                // var httpContent = new StringContent(contentString, Encoding.UTF8, "application/json");
                // var response = client.PostAsync("smtp/email", httpContent).Result;
				var response = client.PostAsJsonAsync("smtp/email", senfinBlueMessage).Result;
                return response;
            });

            return result;
        }

        public Models.EmailEventReport GetMessageHistory(string messageId)
        {
            var result = Execute<Models.EmailEventReport>(client =>
            {
                return client.GetAsync($"smtp/statistics/events?messageId={messageId}").Result;
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
                client.BaseAddress = new Uri("https://api.sendinblue.com/v3/");
                client.DefaultRequestHeaders.Add("UserAgent", "QMailer Client");
                client.DefaultRequestHeaders.Add("api-key", m_ApiKey);
                //client.DefaultRequestHeaders.Add("partner-key", m_PartnerKey);
                var response = predicate.Invoke(client);
                response.EnsureSuccessStatusCode();
                var content = response.Content.ReadAsStringAsync().Result;
                result = response.Content.ReadAsAsync<T>().Result;
            }

            return result;
        }
    }
}
