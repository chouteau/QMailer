using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.SendinBlue.Models
{
    [DataContract]
    public partial class SendSmtpEmailAttachment 
    {
        public SendSmtpEmailAttachment()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SendSmtpEmailAttachment" /> class.
        /// </summary>
        /// <param name="url">Absolute url of the attachment (no local file)..</param>
        /// <param name="content">Base64 encoded chunk data of the attachment generated on the fly.</param>
        /// <param name="name">Required if content is passed. Name of the attachment.</param>
        public SendSmtpEmailAttachment(string url = default(string), byte[] content = default(byte[]), string name = default(string))
        {
            this.Url = url;
            this.Content = content;
            this.Name = name;
        }

        /// <summary>
        /// Absolute url of the attachment (no local file).
        /// </summary>
        /// <value>Absolute url of the attachment (no local file).</value>
        [DataMember(Name = "url", EmitDefaultValue = false)]
        public string Url { get; set; }

        /// <summary>
        /// Base64 encoded chunk data of the attachment generated on the fly
        /// </summary>
        /// <value>Base64 encoded chunk data of the attachment generated on the fly</value>
        [DataMember(Name = "content", EmitDefaultValue = false)]
        public byte[] Content { get; set; }

        /// <summary>
        /// Required if content is passed. Name of the attachment
        /// </summary>
        /// <value>Required if content is passed. Name of the attachment</value>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

    }
}
