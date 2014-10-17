using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using S22.Imap;

namespace QMailer
{
	public class ImapClientService
	{
		private AutoResetEvent m_Reconnect = new AutoResetEvent(false);
		private bool m_Exit = false;
		private ImapClient m_ImapClient;

		public ImapClientService(ILogger logger, Ariane.IServiceBus bus)
		{
			this.Logger = logger;
			this.Bus = bus;
		}

		protected ILogger Logger { get; private set; }
		protected Ariane.IServiceBus Bus { get; private set; }

		public void Start()
		{
			while(!m_Exit)
			{
				InitializeClient();
				m_Reconnect.WaitOne();
				m_Reconnect.Reset();
			}
			if (m_Reconnect != null)
			{
				m_Reconnect.Dispose();
			}
		}

		public void Stop()
		{
			m_Exit = true;
			if (m_Reconnect != null)
			{
				m_Reconnect.Dispose();
			}
		}

		private void InitializeClient()
		{
			var authMethod =  (AuthMethod) Enum.Parse(typeof(AuthMethod), GlobalConfiguration.Configuration.ImapAuthMethod, true);
			m_ImapClient = new ImapClient(GlobalConfiguration.Configuration.ImapHostName, GlobalConfiguration.Configuration.ImapPort, GlobalConfiguration.Configuration.ImapUserName, GlobalConfiguration.Configuration.ImapPassword, authMethod, GlobalConfiguration.Configuration.ImapUseSSL);
			m_ImapClient.NewMessage += result_NewMessage;
			m_ImapClient.IdleError += result_IdleError;
		}

		void result_IdleError(object sender, IdleErrorEventArgs e)
		{
			System.Threading.Thread.Sleep(500);
			Logger.Warn("Imap {0} disconnected", GlobalConfiguration.Configuration.ImapHostName);
			m_Reconnect.Set();
		}

		void result_NewMessage(object sender, IdleMessageEventArgs e)
		{
			try
			{
				var emailMessage = m_ImapClient.GetMessage(e.MessageUID, false);
				var message = new EmailMessage(emailMessage);
				Bus.Send(GlobalConfiguration.Configuration.ReceiveMessageQueueName, message);
			}
			catch(Exception ex)
			{
				Logger.Error(ex);
			}
		}

	}
}
