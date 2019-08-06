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
		public SendEmailMessageReader(IEmailMessageSender sender)
		{
			this.EmailMessageSender = sender;
		}

		protected IEmailMessageSender EmailMessageSender { get; private set; }

		public override void ProcessMessage(EmailMessage message)
		{
            var interceptorTypeList = QMailerService.GetInterceptorTypeList();
            if (interceptorTypeList.Any())
            {
                foreach (var typeInterceptor in interceptorTypeList)
                {
                    try
                    {
                        var interceptor = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeInterceptor) as IMessageInterceptor;
                        message = interceptor.ExecuteInterceptor(message);
                    }
                    catch (Exception e)
                    {
                        GlobalConfiguration.Configuration.Logger.Error(e);
                    }
                  
                }
            }

			GlobalConfiguration.Configuration.Logger.Debug($"receive message to send {message.Subject}");
			EmailMessageSender.Send(message);
		}
	}
}
