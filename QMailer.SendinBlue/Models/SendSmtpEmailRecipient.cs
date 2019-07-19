using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.SendinBlue.Models
{
    [DataContract]
    public partial class SendSmtpEmailRecipient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendSmtpEmailRecipient" /> class.
        /// </summary>
        
        public SendSmtpEmailRecipient() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SendSmtpEmailRecipient" /> class.
        /// </summary>
        /// <param name="name">Name of the sender from which the emails will be sent.</param>
        /// <param name="email">Email of the sender from which the emails will be sent (required).</param>
        //public SendSmtpEmailRecipient(string name = default(string), string email = default(string))
        //{
        //    // to ensure "email" is required (not null)
        //    if (email == null)
        //    {
        //        throw new ArgumentNullException("email is a required property for SendSmtpEmailSender and cannot be null");
        //    }
        //    else
        //    {
        //        this.Email = email;
        //    }
        //    this.Name = name;
        //}

        /// <summary>
        /// Name of the sender from which the emails will be sent
        /// </summary>
        /// <value>Name of the sender from which the emails will be sent</value>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        /// <summary>
        /// Email of the sender from which the emails will be sent
        /// </summary>
        /// <value>Email of the sender from which the emails will be sent</value>
        [DataMember(Name = "email", EmitDefaultValue = false)]
        public string Email { get; set; }

       
    }
}
