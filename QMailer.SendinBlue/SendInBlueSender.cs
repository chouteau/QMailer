﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QMailer.SendinBlue
{
    public class SendInBlueSender : QMailer.IEmailMessageSender
    {
        public SendInBlueSender(Ariane.IServiceBus bus)
        {
            this.Bus = bus;
            this.ApiKey = System.Configuration.ConfigurationManager.AppSettings["SendInBlueApikey"];
        }

        internal Ariane.IServiceBus Bus { get; set; }
        public string ApiKey { get; set; }

        public void Send(EmailMessage message)
        {
            var client = new SendInBlueClient(ApiKey);
            Models.CreateSmtpEmail result = null;
            var content = client.CreateSendinBlueMessage(message);
            try
            {
                result = client.SendMessage(content);
            }
            catch (Exception ex)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                ex.Data.Add("MessageContent", json);

                var sendinBlueMessageJson = Newtonsoft.Json.JsonConvert.SerializeObject(content);
                ex.Data.Add("SendinBlueMessageContent", sendinBlueMessageJson);

                ex.Data.Add("Subject", message.Subject);
                try
                {
                    if (message.Recipients != null)
                    {
                        var recipient = message.Recipients.FirstOrDefault(i => i.SendingType == EmailSendingType.To);
                        if (recipient == null)
                        {
                            recipient = message.Recipients.FirstOrDefault();
                        }
                        if (recipient != null)
                        {
                            ex.Data.Add("Email", recipient.Address);
                        }
                    }
                }
                catch(Exception subEx)
                {
                    GlobalConfiguration.Configuration.Logger.Error(subEx);
                }
                
                GlobalConfiguration.Configuration.Logger.Error(ex);

                var sentFail = new SentFail();
                sentFail.Message = GetFirstGoodErrorMessage(ex);
                sentFail.Stack = ex.ToString();
                sentFail.MessageId = message.MessageId;
                sentFail.Recipients = message.Recipients;
                sentFail.Subject = message.Subject;

                var failQueueName = message.SentFailQueueName ?? GlobalConfiguration.Configuration.SentFailQueueName;
                Bus.Send(failQueueName, sentFail);
            }

            if (result == null)
            {
                return;
            }

            if (!message.DoNotTrack)
            {
                var sentMessage = new SentMessage();
                sentMessage.Body = message.Body;
                sentMessage.MessageId = message.MessageId;
                sentMessage.Recipients = message.Recipients;
                sentMessage.Subject = message.Subject;
                sentMessage.SmtpInfo = "sendinblueapi";
                sentMessage.Sender = message.Sender;
                sentMessage.EntityId = message.EntityId;
                sentMessage.EntityName = message.EntityName;

                if (message.SenderAlias != null)
                {
                    sentMessage.Sender = message.SenderAlias;
                }

                var queueName = message.SentMessageQueueName ?? GlobalConfiguration.Configuration.SentMessageQueueName;
                if (message.SentMessage == null)
                {
                    Bus.Send(queueName, sentMessage);
                }
                else
                {
                    Bus.Send(queueName, message.SentMessage);
                }
            }

            var pending = new Models.PendingMessage();
            pending.MessageId = message.MessageId;
            pending.SendInBlueMessageId = result.MessageId;
			pending.SendDate = DateTime.Now;
			pending.Subject = message.Subject;
            pending.Recipients = message.Recipients;

            StatusChecker.Current.Add(pending);

        }

        private string GetFirstGoodErrorMessage(Exception ex)
        {
            var x = ex;
            var loop = 0;
            while (true)
            {
                if (x.InnerException == null)
                {
                    break;
                }
                if (loop > 10)
                {
                    break;
                }
                x = x.InnerException;
                loop++;
            }
            return x.Message;
        }
    }
}
