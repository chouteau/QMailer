using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using RestSharp.Extensions;

namespace QMailer.Client
{
	public class RestSharpClient
	{
		public RestSharpClient()
		{
		}

		public RestSharp.RestRequest CreateRequest(string resource, RestSharp.Method method)
		{
			var request = new RestSharp.RestRequest(resource, method);
			request.RequestFormat = RestSharp.DataFormat.Json;
			request.XmlSerializer.DateFormat = "yyyy-MM-ddTHH:mm:ss.fff";
			request.JsonSerializer.DateFormat = "yyyy-MM-ddTHH:mm:ss.fff";
			request.AddHeader("apikey", GlobalConfiguration.Configuration.ApiKey);
			request.AddHeader("Accept", "application/json, text/json, text/x-json");
			request.Timeout = 1000 * 30;
			return request;
		}

		public void ExecuteAsync<T>(RestSharp.RestRequest request, Action<T, Exception> callBack)
		{
			var client = CreateHttpClient();

			T result = default(T);
			Exception ex = null;
			var h = client.ExecuteAsync<T>(request, (response, handle) =>
				{
					if (response.StatusCode == System.Net.HttpStatusCode.OK)
					{
						if (response.ErrorException != null)
						{
							ex = response.ErrorException;
						}
						else
						{
							result = response.Data;
						}
					}
					else
					{
						ex = CreateException(response);
					}
					callBack.Invoke(result, ex);
				});
		}

		public void Execute(RestSharp.RestRequest request)
		{
			var client = CreateHttpClient();

			Exception ex = null;
			var response = client.Execute(request);

			if (response.StatusCode != System.Net.HttpStatusCode.OK
				&& response.StatusCode != HttpStatusCode.NoContent)
			{
				ex = CreateException(response);
			}

			if (ex != null)
			{
				throw ex;
			}
		}

		public T Execute<T>(RestSharp.RestRequest request)
		{
			var client = CreateHttpClient();

			T result = default(T);
			Exception ex = null;
			var response = client.Execute(request);

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				if (response.ErrorException != null)
				{
					ex = response.ErrorException;
				}
				else if (response.ContentType.IndexOf("application/json") != -1)
				{
					result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response.Content);
				}
				else if (response.ContentType.IndexOf("text/plain") != -1)
				{
					result = (T) System.Convert.ChangeType(response.Content, typeof(T));
				}
				else if (response.ContentType == "")
				{
					ex = CreateException(response);
				}
				else
				{
					var d = new RestSharp.Deserializers.XmlDeserializer();
					result = d.Deserialize<T>(response);
				}
			}
			else
			{
				ex = CreateException(response);
			}

			if (ex != null)
			{
				throw ex; 
			}

			return result;
		}

		public void ExecuteAsync(RestSharp.RestRequest request, System.IO.StreamWriter writer)
		{
			var client = CreateHttpClient();

			var h = client.ExecuteAsync(request, (response, handle) =>
				{
					var responseSream = response.RawBytes;

					var content = System.Text.Encoding.Default.GetString(responseSream);
					writer.Write(content);
				});
		}

		#region Helpers

		private RestSharp.RestClient CreateHttpClient()
		{
			string address = GetAddressBase();
			var client = new RestSharp.RestClient(address);
			client.UserAgent = GlobalConfiguration.Configuration.UserAgent;
			return client;
		}

		private string GetAddressBase()
		{
			string address = GlobalConfiguration.Configuration.ApiUrl;
			if (!address.StartsWith("http://"))
			{
				address = "http://" + address;
			}
			return address;
		}

		private Exception CreateException(RestSharp.IRestResponse response)
		{
			var ex = new Exception();

			ex.Data.Add("StatusCode", response.StatusCode);
			ex.Data.Add("Content", response.Content);
			ex.Data.Add("ContentType", response.ContentType);

			return ex;
		}

		private void ReadResponseStream(System.IO.Stream rspStream, byte[] readBuffer, System.IO.StreamWriter streamWriter)
		{
			System.Threading.Tasks.Task.Factory.FromAsync<byte[], int, int, int>(rspStream.BeginRead, rspStream.EndRead, readBuffer, 0, readBuffer.Length, state: null).ContinueWith(
				(readTask) =>
				{
					if (readTask.IsCanceled)
					{
						return;
					}
					if (readTask.IsFaulted)
					{
						throw readTask.Exception;
					}

					int bytesRead = readTask.Result;
					string content = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);
					Console.WriteLine("Received: {0}", content);
					streamWriter.Write(content);

					if (bytesRead != 0)
					{
						ReadResponseStream(rspStream, readBuffer, streamWriter);
					}
				});
		}

		#endregion
	}
}
