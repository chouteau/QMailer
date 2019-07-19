using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.SendinBlue.Models
{
    [DataContract]
    public partial class CreateSmtpEmail 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSmtpEmail" /> class.
        /// </summary>
        public CreateSmtpEmail() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSmtpEmail" /> class.
        /// </summary>
        /// <param name="messageId">Message ID of the SMTP Email sent (required).</param>
        //public CreateSmtpEmail(string messageId = default(string))
        //{
        //    // to ensure "messageId" is required (not null)
        //    if (messageId == null)
        //    {
        //        throw new ArgumentNullException("messageId is a required property for CreateSmtpEmail and cannot be null");
        //    }
        //    else
        //    {
        //        this.MessageId = messageId;
        //    }
        //}

        /// <summary>
        /// Message ID of the SMTP Email sent
        /// </summary>
        /// <value>Message ID of the SMTP Email sent</value>
        [DataMember(Name = "messageId", EmitDefaultValue = false)]
        public string MessageId { get; set; }

    }
}
