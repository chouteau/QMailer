using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QMailer.MailJet
{
	public class StatusChecker
	{
		private static Lazy<StatusChecker> m_LazyInstance = new Lazy<StatusChecker>(() =>
		{
			return new StatusChecker();
		}, false);

		private ManualResetEvent m_Terminate = new ManualResetEvent(false);
		private bool m_Terminated = false;
		private Thread m_CheckThread;

		private StatusChecker()
		{
			PendingList = new System.Collections.Concurrent.ConcurrentDictionary<string,PendingMessage>();
			QMailer.QMailerService.OnStop(() => Stop());

			m_CheckThread = new Thread(new ThreadStart(CheckMessages));
			m_CheckThread.Name = "CheckPendingMailJetEmailMessages";
			m_CheckThread.IsBackground = true;
			m_CheckThread.Start();

			this.PublicKey = System.Configuration.ConfigurationManager.AppSettings["MailJetPublicKey"];
			this.PrivateKey = System.Configuration.ConfigurationManager.AppSettings["MailJetPrivateKey"];

			Bus = GlobalConfiguration.Configuration.DependencyResolver.GetService<Ariane.IServiceBus>();
		}

		private System.Collections.Concurrent.ConcurrentDictionary<string,PendingMessage> PendingList { get; set; }
		private string PublicKey { get; set; }
		private string PrivateKey { get; set; }

		private Ariane.IServiceBus Bus { get; set; }


		public static StatusChecker Current
		{
			get
			{
				return m_LazyInstance.Value;
			}
		}

		public void Stop()
		{
			m_Terminated = true;
			m_Terminate.Set();

			if (m_Terminate != null)
			{
				if (!m_Terminate.SafeWaitHandle.IsClosed)
				{
					m_Terminate.Set();
				}
				m_Terminate.Dispose();
			}
			if (m_CheckThread != null)
			{
				if (!m_CheckThread.Join(TimeSpan.FromSeconds(5)))
				{
					m_CheckThread.Abort();
				}
			}
		}

		public void Add(PendingMessage pending)
		{
			if (PendingList.ContainsKey(pending.MessageId))
			{
				return;
			}
			PendingList.TryAdd(pending.MessageId, pending);
		}

		public void CheckMessages()
		{
			GlobalConfiguration.Configuration.Logger.Info("Check pending started");

			while (!m_Terminated)
			{
				var waitHandles = new WaitHandle[] { m_Terminate };
				int result = ManualResetEvent.WaitAny(waitHandles, 60 * 1000, false);
				if (result == 0)
				{
					m_Terminated = true;
					break;
				}

				if (PendingList.Count == 0)
				{
					continue;
				}

				GlobalConfiguration.Configuration.Logger.Info($"Check sent for {PendingList.Count} emails");

				var messageToRemoveList = new List<string>();
				foreach (var item in PendingList.Keys)
				{
					try
					{
						System.Threading.Thread.Sleep(5 * 500);
						var messageId = CheckPendingMessage(item);
						if (messageId != null)
						{
							messageToRemoveList.Add(messageId);
						}
					}
					catch(Exception ex)
					{
						GlobalConfiguration.Configuration.Logger.Error(ex);
					}
				}

				foreach (var item in messageToRemoveList)
				{
					PendingMessage pm = null;
					PendingList.TryRemove(item, out pm);
				}
			}
		}

		public string CheckPendingMessage(string messageId)
		{
			PendingMessage pending = null;
			PendingList.TryGetValue(messageId, out pending);
			if (pending == null)
			{
				return null;
			}

			if (pending.CheckCount > 10)
			{
				return pending.MessageId;
			}

			// Every 20 minutes or first time
			if (!pending.LastCheckDate.HasValue
				|| pending.LastCheckDate.Value.AddMinutes(20) < DateTime.Now)
			{
				var client = new MailJetClient(PublicKey, PrivateKey);
				pending.CheckCount++;
				pending.LastCheckDate = DateTime.Now;
				Models.MessageHistory result = null;
				try
				{
					result = client.GetMessageHistory(pending.MailJetMessageId);
				}
				catch(Exception ex)
				{
					GlobalConfiguration.Configuration.Logger.Error($"Get history fail with email {pending.Email}/{pending.Subject}");
					GlobalConfiguration.Configuration.Logger.Error(ex);
					SentFail(pending, "mailjet:historyFail", "Status code error");
				}

				if (result == null)
				{
					return null;
				}

				if (result.Count == 0)
				{
					return null;
				}

				var item = result.Data.Last();

				if (item.EventType == Models.MessageStatus.Clicked
					|| item.EventType == Models.MessageStatus.Opened
					|| item.EventType == Models.MessageStatus.Sent)
				{
					GlobalConfiguration.Configuration.Logger.Debug($"email {pending.Email}/{pending.Subject} was sent with status {item.EventType}");
					return pending.MessageId;
				}
				else if (item.EventType == Models.MessageStatus.Bounced)
				{
					GlobalConfiguration.Configuration.Logger.Warn($"email {pending.Email}/{pending.Subject} was bounces");
					SentFail(pending, "mailjet:bounce", item.Comment);
					return pending.MessageId;
				}
				else if (item.EventType == Models.MessageStatus.Blocked)
				{
					GlobalConfiguration.Configuration.Logger.Warn($"email {pending.Email}/{pending.Subject} was blocked");
					SentFail(pending, "mailjet:blocked");
				}
				else if (item.EventType == Models.MessageStatus.Queued)
				{
					return null;
				}
				else if (item.EventType == Models.MessageStatus.Spam)
				{
					GlobalConfiguration.Configuration.Logger.Warn($"email {pending.Email}/{pending.Subject} marked as spam");
					SentFail(pending, "mailjet:spam");
					return pending.MessageId;
				}
				else if (item.EventType == Models.MessageStatus.Unsub)
				{
					GlobalConfiguration.Configuration.Logger.Warn($"email {pending.Email}/{pending.Subject} need unsubscribe");
					SentFail(pending, "mailjet:unsubscribe");
					return pending.MessageId;
				}
			}

			return null;
		}

		public void SentFail(PendingMessage pending, string status, string message = null)
		{
			var sentFail = new SentFail();
			sentFail.Stack = status;
			sentFail.Message = message;
			sentFail.MessageId = pending.MessageId;
			sentFail.Recipients = pending.Recipients;
			sentFail.Subject = pending.Subject;

			var contentString = Newtonsoft.Json.JsonConvert.SerializeObject(sentFail);

			var path = System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location);
			path = System.IO.Path.Combine(path, $"fails/{status.Replace("mailjet:","")}-{Guid.NewGuid()}.json");
			System.IO.File.WriteAllText(path, contentString);

			var failQueueName = GlobalConfiguration.Configuration.SentFailQueueName;
			Bus.Send(failQueueName, sentFail);
		}
	}
}
