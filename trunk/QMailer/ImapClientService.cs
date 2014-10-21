using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using S22.Imap;

namespace QMailer
{
	public class ImapClientService : IDisposable
	{
		private AutoResetEvent m_Reconnect;
		private AutoResetEvent m_EventStop;
		private bool m_Exit = false;
		private ImapClient m_ImapClient;
		private System.Threading.Thread m_Thread;
		private DateTime m_LastCheckDate;

		public ImapClientService(ILogger logger, Ariane.IServiceBus bus)
		{
			this.Logger = logger;
			this.Bus = bus;
			this.m_LastCheckDate = DateTime.Now.AddMinutes(30);
		}

		protected ILogger Logger { get; private set; }
		protected Ariane.IServiceBus Bus { get; private set; }

		public uint? LastMessageId { get; set; }

		public void Start()
		{
			m_Reconnect = new AutoResetEvent(false);
			m_EventStop = new AutoResetEvent(false);

			m_Thread = new Thread(HandleNewEmail);
			m_Thread.IsBackground = true;
			m_Thread.Name = "QMailerImapClientHandleNewEmail";
			m_Thread.Priority = ThreadPriority.Lowest;

			m_Thread.Start();
		}

		private void HandleNewEmail()
		{
			InitializeClient();
			while (!m_Exit)
			{
				CheckMailBox();
				var waitHandles = new WaitHandle[] { m_EventStop, m_Reconnect };
				int result = ManualResetEvent.WaitAny(waitHandles, 30 * 1000, true);
				if (result == 0)
				{
					m_Exit = true;
					break;
				}
				else if (result == 1) // Reconnect
				{
					InitializeClient();
				}
				m_LastCheckDate = DateTime.Now;
			}
			if (m_Reconnect != null)
			{
				m_Reconnect.Dispose();
			}
		}

		public void Stop()
		{
			m_Exit = true;
			if (m_EventStop != null)
			{
				m_EventStop.Set();
			}
			if (m_Thread != null && !m_Thread.Join(TimeSpan.FromSeconds(5)))
			{
				m_Thread.Abort();
			}
		}


		private void CheckMailBox()
		{
			SearchCondition search = null;
			if (LastMessageId.HasValue)
			{
				search = SearchCondition.GreaterThan(LastMessageId.Value);
			}
			else
			{
				search = SearchCondition.SentSince(m_LastCheckDate);
			}
			var list = m_ImapClient.Search(search);
			foreach (var uid in list)
			{
				var emailMessage = m_ImapClient.GetMessage(uid);
				var message = new EmailMessage(emailMessage);
				message.ImapMessageId = uid.ToString();
				LastMessageId = uid;
				Bus.Send(GlobalConfiguration.Configuration.ReceiveMessageQueueName, message);
			}
		}

		private void InitializeClient()
		{
			var authMethod =  (AuthMethod) Enum.Parse(typeof(AuthMethod), GlobalConfiguration.Configuration.ImapAuthMethod, true);
			m_ImapClient = new ImapClient(GlobalConfiguration.Configuration.ImapHostName, GlobalConfiguration.Configuration.ImapPort, GlobalConfiguration.Configuration.ImapUserName, GlobalConfiguration.Configuration.ImapPassword, authMethod, GlobalConfiguration.Configuration.ImapUseSSL);
			m_ImapClient.IdleError += result_IdleError;
		}

		void result_IdleError(object sender, IdleErrorEventArgs e)
		{
			System.Threading.Thread.Sleep(500);
			Logger.Warn("Imap {0} disconnected", GlobalConfiguration.Configuration.ImapHostName);
			m_Reconnect.Set();
		}

		public void Dispose()
		{
			if (m_EventStop != null)
			{
				m_EventStop.Dispose();
			}
			if (m_Reconnect != null)
			{
				m_Reconnect.Dispose();
			}
		}
	}
}
