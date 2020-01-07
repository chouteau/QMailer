using QMailer.SendinBlue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QMailer.SendinBlue
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
            PendingList = new System.Collections.Concurrent.ConcurrentDictionary<string, PendingMessage>();
            QMailer.QMailerService.OnStop(() => Stop());

            m_CheckThread = new Thread(new ThreadStart(CheckMessages));
            m_CheckThread.Name = "CheckPendingMailJetEmailMessages";
            m_CheckThread.IsBackground = true;
            m_CheckThread.Start();

            this.ApiKey = System.Configuration.ConfigurationManager.AppSettings["SendInBlueApikey"];

            Bus = GlobalConfiguration.Configuration.DependencyResolver.GetService<Ariane.IServiceBus>();
        }

        private System.Collections.Concurrent.ConcurrentDictionary<string, PendingMessage> PendingList { get; set; }
        private string ApiKey { get; set; }

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
                    catch (Exception ex)
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
                var client = new SendInBlueClient(ApiKey);
                pending.CheckCount++;
                pending.LastCheckDate = DateTime.Now;
                Models.EmailEventReport result = null;
                try
                {
                    result = client.GetMessageHistory(pending.SendInBlueMessageId);
                }
                catch (Exception ex)
                {
                    GlobalConfiguration.Configuration.Logger.Error($"Get history fail with email {pending.Email}/{pending.Subject}");
                    GlobalConfiguration.Configuration.Logger.Error(ex);
                    SentFail(pending, "sendinblue:historyFail", "Status code error");
                }

                if (result == null)
                {
                    return null;
                }

                if (result.Events == null || result.Events.Count == 0)
                {
                    return null;
                }

                var item = result.Events.Last();

                if (item.Event == EmailEventReportEvent.EventEnum.Clicks
                    || item.Event == EmailEventReportEvent.EventEnum.Opened
                    || item.Event == EmailEventReportEvent.EventEnum.Delivered)
                {
                    GlobalConfiguration.Configuration.Logger.Debug($"email {pending.Email}/{pending.Subject} was sent with status {item.Event}");
                    return pending.MessageId;
                }
                else if (item.Event == EmailEventReportEvent.EventEnum.Bounces
                    || item.Event == EmailEventReportEvent.EventEnum.HardBounces
                    || item.Event == EmailEventReportEvent.EventEnum.SoftBounces)
                {
                    GlobalConfiguration.Configuration.Logger.Warn($"email {pending.Email}/{pending.Subject} was bounces");
                    SentFail(pending, "sendinblue:bounce", item.Reason);
                    return pending.MessageId;
                }
                else if (item.Event == EmailEventReportEvent.EventEnum.Blocked)
                {
                    GlobalConfiguration.Configuration.Logger.Warn($"email {pending.Email}/{pending.Subject} was blocked");
                    SentFail(pending, "sendinblue:blocked");
                }
                else if (item.Event == EmailEventReportEvent.EventEnum.Requests)
                {
                    return null;
                }
                else if (item.Event == EmailEventReportEvent.EventEnum.Spam)
                {
                    GlobalConfiguration.Configuration.Logger.Warn($"email {pending.Email}/{pending.Subject} marked as spam");
                    SentFail(pending, "sendinblue:spam");
                    return pending.MessageId;
                }
                else if (item.Event == EmailEventReportEvent.EventEnum.Unsubscribed)
                {
                    GlobalConfiguration.Configuration.Logger.Warn($"email {pending.Email}/{pending.Subject} need unsubscribe");
                    SentFail(pending, "sendinblue:unsubscribe");
                    return pending.MessageId;
                }
                else if (item.Event == EmailEventReportEvent.EventEnum.Invalid)
                {
                    GlobalConfiguration.Configuration.Logger.Warn($"email {pending.Email}/{pending.Subject} was invalid");
                    SentFail(pending, "sendinblue:invalid");
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
            path = System.IO.Path.Combine(path, $"fails/{status.Replace("sendinblue:", "")}-{Guid.NewGuid()}.json");
            System.IO.File.WriteAllText(path, contentString);

            var failQueueName = GlobalConfiguration.Configuration.SentFailQueueName;
            Bus.Send(failQueueName, sentFail);
        }
    }
}
