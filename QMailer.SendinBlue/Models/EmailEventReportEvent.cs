using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.SendinBlue.Models
{
    [DataContract]
    public partial class EmailEventReportEvent
    {
        /// <summary>
        /// Event which occurred
        /// </summary>
        /// <value>Event which occurred</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum EventEnum
        {

            /// <summary>
            /// Enum Bounces for value: bounces
            /// </summary>
            [EnumMember(Value = "bounces")]
            Bounces = 1,

            /// <summary>
            /// Enum HardBounces for value: hardBounces
            /// </summary>
            [EnumMember(Value = "hardBounces")]
            HardBounces = 2,

            /// <summary>
            /// Enum SoftBounces for value: softBounces
            /// </summary>
            [EnumMember(Value = "softBounces")]
            SoftBounces = 3,

            /// <summary>
            /// Enum Delivered for value: delivered
            /// </summary>
            [EnumMember(Value = "delivered")]
            Delivered = 4,

            /// <summary>
            /// Enum Spam for value: spam
            /// </summary>
            [EnumMember(Value = "spam")]
            Spam = 5,

            /// <summary>
            /// Enum Requests for value: requests
            /// </summary>
            [EnumMember(Value = "requests")]
            Requests = 6,

            /// <summary>
            /// Enum Opened for value: opened
            /// </summary>
            [EnumMember(Value = "opened")]
            Opened = 7,

            /// <summary>
            /// Enum Clicks for value: clicks
            /// </summary>
            [EnumMember(Value = "clicks")]
            Clicks = 8,

            /// <summary>
            /// Enum Invalid for value: invalid
            /// </summary>
            [EnumMember(Value = "invalid")]
            Invalid = 9,

            /// <summary>
            /// Enum Deferred for value: deferred
            /// </summary>
            [EnumMember(Value = "deferred")]
            Deferred = 10,

            /// <summary>
            /// Enum Blocked for value: blocked
            /// </summary>
            [EnumMember(Value = "blocked")]
            Blocked = 11,

            /// <summary>
            /// Enum Unsubscribed for value: unsubscribed
            /// </summary>
            [EnumMember(Value = "unsubscribed")]
            Unsubscribed = 12
        }

        /// <summary>
        /// Event which occurred
        /// </summary>
        /// <value>Event which occurred</value>
        [DataMember(Name = "event", EmitDefaultValue = false)]
        public EventEnum Event { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailEventReportEvent" /> class.
        /// </summary>
        public EmailEventReportEvent() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetEmailEventReportEvents" /> class.
        /// </summary>
        /// <param name="email">Email address which generates the event (required).</param>
        /// <param name="date">UTC date-time on which the event has been generated (required).</param>
        /// <param name="subject">Subject of the event.</param>
        /// <param name="messageId">Message ID which generated the event (required).</param>
        /// <param name="_event">Event which occurred (required).</param>
        /// <param name="reason">Reason of bounce (only available if the event is hardbounce or softbounce).</param>
        /// <param name="tag">Tag of the email which generated the event.</param>
        /// <param name="ip">IP from which the user has opened the email or clicked on the link (only available if the event is opened or clicks).</param>
        /// <param name="link">The link which is sent to the user (only available if the event is requests or opened or clicks).</param>
        /// <param name="from">Sender email from which the emails are sent.</param>
        //public EmailEventReportEvent(string email = default(string), DateTime? date = default(DateTime?), string subject = default(string), string messageId = default(string), EventEnum _event = default(EventEnum), string reason = default(string), string tag = default(string), string ip = default(string), string link = default(string), string from = default(string))
        //{
        //    // to ensure "email" is required (not null)
        //    if (email == null)
        //    {
        //        throw new ArgumentNullException("email is a required property for GetEmailEventReportEvents and cannot be null");
        //    }
        //    else
        //    {
        //        this.Email = email;
        //    }
        //    // to ensure "date" is required (not null)
        //    if (date == null)
        //    {
        //        throw new ArgumentNullException("date is a required property for GetEmailEventReportEvents and cannot be null");
        //    }
        //    else
        //    {
        //        this.Date = date;
        //    }
        //    // to ensure "messageId" is required (not null)
        //    if (messageId == null)
        //    {
        //        throw new ArgumentNullException("messageId is a required property for GetEmailEventReportEvents and cannot be null");
        //    }
        //    else
        //    {
        //        this.MessageId = messageId;
        //    }
        //    // to ensure "_event" is required (not null)
        //    if (_event == null)
        //    {
        //        throw new ArgumentNullException("_event is a required property for GetEmailEventReportEvents and cannot be null");
        //    }
        //    else
        //    {
        //        this.Event = _event;
        //    }
        //    this.Subject = subject;
        //    this.Reason = reason;
        //    this.Tag = tag;
        //    this.Ip = ip;
        //    this.Link = link;
        //    this.From = from;
        //}

        /// <summary>
        /// Email address which generates the event
        /// </summary>
        /// <value>Email address which generates the event</value>
        [DataMember(Name = "email", EmitDefaultValue = false)]
        public string Email { get; set; }

        /// <summary>
        /// UTC date-time on which the event has been generated
        /// </summary>
        /// <value>UTC date-time on which the event has been generated</value>
        [DataMember(Name = "date", EmitDefaultValue = false)]
        public DateTime? Date { get; set; }

        /// <summary>
        /// Subject of the event
        /// </summary>
        /// <value>Subject of the event</value>
        [DataMember(Name = "subject", EmitDefaultValue = false)]
        public string Subject { get; set; }

        /// <summary>
        /// Message ID which generated the event
        /// </summary>
        /// <value>Message ID which generated the event</value>
        [DataMember(Name = "messageId", EmitDefaultValue = false)]
        public string MessageId { get; set; }


        /// <summary>
        /// Reason of bounce (only available if the event is hardbounce or softbounce)
        /// </summary>
        /// <value>Reason of bounce (only available if the event is hardbounce or softbounce)</value>
        [DataMember(Name = "reason", EmitDefaultValue = false)]
        public string Reason { get; set; }

        /// <summary>
        /// Tag of the email which generated the event
        /// </summary>
        /// <value>Tag of the email which generated the event</value>
        [DataMember(Name = "tag", EmitDefaultValue = false)]
        public string Tag { get; set; }

        /// <summary>
        /// IP from which the user has opened the email or clicked on the link (only available if the event is opened or clicks)
        /// </summary>
        /// <value>IP from which the user has opened the email or clicked on the link (only available if the event is opened or clicks)</value>
        [DataMember(Name = "ip", EmitDefaultValue = false)]
        public string Ip { get; set; }

        /// <summary>
        /// The link which is sent to the user (only available if the event is requests or opened or clicks)
        /// </summary>
        /// <value>The link which is sent to the user (only available if the event is requests or opened or clicks)</value>
        [DataMember(Name = "link", EmitDefaultValue = false)]
        public string Link { get; set; }

        /// <summary>
        /// Sender email from which the emails are sent
        /// </summary>
        /// <value>Sender email from which the emails are sent</value>
        [DataMember(Name = "from", EmitDefaultValue = false)]
        public string From { get; set; }

    }
}
