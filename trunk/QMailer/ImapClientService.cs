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
			while (!m_Exit)
			{
				try
				{
					CheckMailBox();
				}
				catch(Exception ex)
				{
					Logger.Error(ex);
				}
				var waitHandles = new WaitHandle[] { m_EventStop, m_Reconnect };
				int result = ManualResetEvent.WaitAny(waitHandles, 5 * 60 * 1000, true);
				if (result == 0)
				{
					m_Exit = true;
					break;
				}
				m_LastCheckDate = DateTime.Now;
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
			var authMethod = (AuthMethod)Enum.Parse(typeof(AuthMethod), GlobalConfiguration.Configuration.ImapAuthMethod, true);
			m_ImapClient = new ImapClient(GlobalConfiguration.Configuration.ImapHostName, GlobalConfiguration.Configuration.ImapPort, GlobalConfiguration.Configuration.ImapUserName, GlobalConfiguration.Configuration.ImapPassword, authMethod, GlobalConfiguration.Configuration.ImapUseSSL);

			SearchCondition search = null;
			search = SearchCondition.SentSince(m_LastCheckDate.AddMinutes(-10));
			var list = m_ImapClient.Search(search);
			foreach (var uid in list)
			{
				m_LastCheckDate = DateTime.Now;
				if (uid <= LastMessageId.GetValueOrDefault(0))
				{
					continue;
				}
				LastMessageId = uid;
				var emailMessage = m_ImapClient.GetMessage(uid, false);
				var message = new EmailMessage(emailMessage);
				message.ImapMessageId = uid.ToString();
				Bus.Send(GlobalConfiguration.Configuration.ReceiveMessageQueueName, message);
			}

			m_ImapClient.Logout();
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
