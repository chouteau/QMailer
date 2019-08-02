using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace QMailer.Client
{
    public class QMailerService
    {
		public QMailerService()
		{

		}

		public EmailConfig CreateEmailConfig(string messageId)
		{
			var result = ExecuteRetry<EmailConfig>(client =>
			{
				return client.GetAsync($"api/qmailer/createemailconfig/{messageId}").Result;
			}, true);
			return result;
		}

		public void Send(EmailConfig emailConfig)
		{
			if (emailConfig.Model == null)
			{
				return;
			}

			emailConfig.AssemblyQualifiedTypeNameModel = emailConfig.Model.GetType().AssemblyQualifiedName;

			ExecuteRetry<object>(client =>
			{
				return client.PostAsJsonAsync<EmailConfig>("api/qmailer/sendemailconfig", emailConfig).Result;
			}, false);
		}

		public EmailMessage GetEmailMessage(EmailConfig emailConfig)
		{
			if (emailConfig == null
				|| emailConfig.Model == null)
			{
				return null;
			}

			emailConfig.AssemblyQualifiedTypeNameModel = emailConfig.Model.GetType().AssemblyQualifiedName;

			var result = ExecuteRetry<EmailMessage>(client =>
			{
				return client.PostAsJsonAsync<EmailConfig>("api/qmailer/emailmessage", emailConfig).Result;
			}, true);

			return result;
		}

		public void Send(EmailMessage emailMessage)
		{
			ExecuteRetry<object>(client =>
			{
				return client.PostAsJsonAsync<EmailMessage>("api/qmailer/sendemailmessage", emailMessage).Result;
			}, false);
		}

		public List<QMailer.Web.TemplateInfo> GetTemplateListByModel(string modelName)
		{
			var result = ExecuteRetry<List<QMailer.Web.TemplateInfo>>(client =>
			{
				return client.GetAsync($"api/qmailer/templatelistbymodel/{modelName}").Result;
			}, true);

			return result;
		}

		public List<QMailer.Web.TemplateInfo> GetAllTemplateList()
		{
			throw new NotImplementedException();
		}

		public void DeleteTemplate(QMailer.Web.TemplateInfo template)
		{
			throw new NotImplementedException();
		}

		public void SaveTemplate(QMailer.Web.TemplateInfo template)
		{
			throw new NotImplementedException();
		}

		public QMailer.Web.TemplateInfo CreateCustomTemplate()
		{
			throw new NotImplementedException();
		}

		public string GetPreviewUrl(QMailer.EmailConfig config)
		{
			if (config.Model == null)
			{
				return null;
			}

			config.AssemblyQualifiedTypeNameModel = config.Model.GetType().AssemblyQualifiedName;

			var result = ExecuteRetry<Newtonsoft.Json.Linq.JObject>(client =>
			{
				return client.PostAsJsonAsync($"api/qmailer/previewkey", config).Result;
			}, true);
			var key = result["messageId"];

			//var key = result.GetType().GetProperty("messageId", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(result, null);
			string url = null;
			if (GlobalConfiguration.Configuration.ApiUrl.StartsWith("http"))
			{
				url = $"{GlobalConfiguration.Configuration.ApiUrl}/qmailer/preview/{key}";
			}
			else
			{
				url = $"http://{GlobalConfiguration.Configuration.ApiUrl}/qmailer/preview/{key}";
			}

			return url;
		}

		private T ExecuteRetry<T>(Func<HttpClient, HttpResponseMessage> predicate, bool hasReturn = false)
		{
			var loop = 0;
			T result = default(T);
			while (true)
			{
				var handler = new HttpClientHandler()
				{
					AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
					AllowAutoRedirect = false,
					UseCookies = true,
				};

				using (var httpClient = new System.Net.Http.HttpClient(handler))
				{
					httpClient.BaseAddress = new Uri(GlobalConfiguration.Configuration.ApiUrl);
					httpClient.DefaultRequestHeaders.Add("apikey", GlobalConfiguration.Configuration.ApiKey);

					Exception x = null;
					HttpResponseMessage response = null;
					try
					{
						response = predicate.Invoke(httpClient);
						response.EnsureSuccessStatusCode();
						if (hasReturn)
						{
							result = response.Content.ReadAsAsync<T>().Result;
						}
						break;
					}
					catch (HttpRequestException hex) when (hex.Message.LastIndexOf("404") != -1)
					{
						x = hex;
						loop = 4;
					}
					catch (Exception ex)
					{
						loop++;
						x = ex;
						while (true)
						{
							if (x.InnerException == null)
							{
								if (x is System.Net.WebException)
								{
									var webex = x as System.Net.WebException;
									if (webex.Status == System.Net.WebExceptionStatus.NameResolutionFailure)
									{
										loop = 4;
									}
								}
								break;
							}
							x = x.InnerException;
						}
						System.Threading.Thread.Sleep(5 * 1000);
					}

					if (loop > 3
						&& x != null)
					{
						throw x;
					}
				}
			}
			return result;
		}

	}
}
