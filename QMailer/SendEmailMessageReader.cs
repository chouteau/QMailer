using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QMailer
{
	public class SendEmailMessageReader : Ariane.MessageReaderBase<EmailMessage>
	{
		private int m_Pool;
		private System.Threading.ManualResetEvent m_WaitingFreeThread;

		public SendEmailMessageReader(ILogger logger)
		{
			this.m_Pool = 0;
			m_WaitingFreeThread = new System.Threading.ManualResetEvent(false);
			this.Logger = logger;
		}

		protected ILogger Logger { get; private set; }

		public override void ProcessMessage(EmailMessage message)
		{
			Logger.Debug("receive message for send by email");
			EmailerService.Current.Send(message);
		}
	}
}
