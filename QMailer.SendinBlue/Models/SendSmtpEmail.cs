using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.SendinBlue.Models
{
    [DataContract]
    public partial class SendSmtpEmail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendSmtpEmail" /> class.
        /// </summary>
        public SendSmtpEmail() {
            //this.To = new List<SendSmtpEmailRecipient>();
            //this.Bcc = new List<SendSmtpEmailRecipient>();
            //this.Cc = new List<SendSmtpEmailRecipient>();
            //this.Tags = new List<string>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SendSmtpEmail" /> class.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="to">List of email addresses and names (optional) of the recipients. For example, [{&#39;name&#39;:&#39;Jimmy&#39;, &#39;email&#39;:&#39;jimmy98@example.com&#39;}, {&#39;name&#39;:&#39;Joe&#39;, &#39;email&#39;:&#39;joe@example.com&#39;}] (required).</param>
        /// <param name="bcc">List of email addresses and names (optional) of the recipients in bcc.</param>
        /// <param name="cc">List of email addresses and names (optional) of the recipients in cc.</param>
        /// <param name="htmlContent">HTML body of the message ( Mandatory if &#39;templateId&#39; is not passed, ignored if &#39;templateId&#39; is passed ).</param>
        /// <param name="textContent">Plain Text body of the message ( Ignored if &#39;templateId&#39; is passed ).</param>
        /// <param name="subject">Subject of the message. Mandatory if &#39;templateId&#39; is not passed.</param>
        /// <param name="replyTo">replyTo.</param>
        /// <param name="attachment">Pass the absolute URL (no local file) or the base64 content of the attachment along with the attachment name (Mandatory if attachment content is passed). For example, [{&#39;url&#39;:&#39;https://attachment.domain.com/myAttachmentFromUrl.jpg&#39;, &#39;name&#39;:&#39;My attachment 1&#39;}, {&#39;content&#39;:&#39;base64 exmaple content&#39;, &#39;name&#39;:&#39;My attachment 2&#39;}]. Allowed extensions for attachment file: xlsx, xls, ods, docx, docm, doc, csv, pdf, txt, gif, jpg, jpeg, png, tif, tiff, rtf, bmp, cgm, css, shtml, html, htm, zip, xml, ppt, pptx, tar, ez, ics, mobi, msg, pub, eps and odt ( If &#39;templateId&#39; is passed and is in New Template Language format then only attachment url is accepted. If template is in Old template Language format, then &#39;attachment&#39; is ignored ).</param>
        /// <param name="headers">Pass the set of headers that shall be sent along the mail headers in the original email. &#39;sender.ip&#39; header can be set (only for dedicated ip users) to mention the IP to be used for sending transactional emails. For example, {&#39;Content-Type&#39;:&#39;text/html&#39;, &#39;charset&#39;:&#39;iso-8859-1&#39;, &#39;sender.ip&#39;:&#39;1.2.3.4&#39;}.</param>
        /// <param name="templateId">Id of the template.</param>
        /// <param name="_params">Pass the set of attributes to customize the template. For example, {&#39;FNAME&#39;:&#39;Joe&#39;, &#39;LNAME&#39;:&#39;Doe&#39;}. It&#39;s considered only if template is in New Template Language format..</param>
        /// <param name="tags">Tag your emails to find them more easily.</param>
        //public SendSmtpEmail(SendSmtpEmailRecipient sender = default(SendSmtpEmailRecipient), List<SendSmtpEmailRecipient> to = default(List<SendSmtpEmailRecipient>), List<SendSmtpEmailRecipient> bcc = default(List<SendSmtpEmailRecipient>), List<SendSmtpEmailRecipient> cc = default(List<SendSmtpEmailRecipient>), string htmlContent = default(string), string textContent = default(string), string subject = default(string), SendSmtpEmailRecipient replyTo = default(SendSmtpEmailRecipient), List<SendSmtpEmailAttachment> attachment = default(List<SendSmtpEmailAttachment>), Object headers = default(Object), long? templateId = default(long?), Object _params = default(Object), List<string> tags = default(List<string>))
        //{
        //    // to ensure "to" is required (not null)
        //    if (to == null)
        //    {
        //        throw new ArgumentNullException("to is a required property for SendSmtpEmail and cannot be null");
        //    }
        //    else
        //    {
        //        this.To = to;
        //    }
        //    this.Sender = sender;
        //    this.Bcc = bcc;
        //    this.Cc = cc;
        //    this.HtmlContent = htmlContent;
        //    this.TextContent = textContent;
        //    this.Subject = subject;
        //    this.ReplyTo = replyTo;
        //    this.Attachment = attachment;
        //    this.Headers = headers;
        //    this.TemplateId = templateId;
        //    this.Params = _params;
        //    this.Tags = tags;
        //}

        /// <summary>
        /// Gets or Sets Sender
        /// </summary>
        [DataMember(Name = "sender", EmitDefaultValue = false)]
        public SendSmtpEmailRecipient Sender { get; set; }

        /// <summary>
        /// List of email addresses and names (optional) of the recipients. For example, [{&#39;name&#39;:&#39;Jimmy&#39;, &#39;email&#39;:&#39;jimmy98@example.com&#39;}, {&#39;name&#39;:&#39;Joe&#39;, &#39;email&#39;:&#39;joe@example.com&#39;}]
        /// </summary>
        /// <value>List of email addresses and names (optional) of the recipients. For example, [{&#39;name&#39;:&#39;Jimmy&#39;, &#39;email&#39;:&#39;jimmy98@example.com&#39;}, {&#39;name&#39;:&#39;Joe&#39;, &#39;email&#39;:&#39;joe@example.com&#39;}]</value>
        [DataMember(Name = "to", EmitDefaultValue = false)]
        public List<SendSmtpEmailRecipient> To { get; set; }

        /// <summary>
        /// List of email addresses and names (optional) of the recipients in bcc
        /// </summary>
        /// <value>List of email addresses and names (optional) of the recipients in bcc</value>
        [DataMember(Name = "bcc", EmitDefaultValue = false)]
        public List<SendSmtpEmailRecipient> Bcc { get; set; }

        /// <summary>
        /// List of email addresses and names (optional) of the recipients in cc
        /// </summary>
        /// <value>List of email addresses and names (optional) of the recipients in cc</value>
        [DataMember(Name = "cc", EmitDefaultValue = false)]
        public List<SendSmtpEmailRecipient> Cc { get; set; }

        /// <summary>
        /// HTML body of the message ( Mandatory if &#39;templateId&#39; is not passed, ignored if &#39;templateId&#39; is passed )
        /// </summary>
        /// <value>HTML body of the message ( Mandatory if &#39;templateId&#39; is not passed, ignored if &#39;templateId&#39; is passed )</value>
        [DataMember(Name = "htmlContent", EmitDefaultValue = false)]
        public string HtmlContent { get; set; }

        /// <summary>
        /// Plain Text body of the message ( Ignored if &#39;templateId&#39; is passed )
        /// </summary>
        /// <value>Plain Text body of the message ( Ignored if &#39;templateId&#39; is passed )</value>
        [DataMember(Name = "textContent", EmitDefaultValue = false)]
        public string TextContent { get; set; }

        /// <summary>
        /// Subject of the message. Mandatory if &#39;templateId&#39; is not passed
        /// </summary>
        /// <value>Subject of the message. Mandatory if &#39;templateId&#39; is not passed</value>
        [DataMember(Name = "subject", EmitDefaultValue = false)]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or Sets ReplyTo
        /// </summary>
        [DataMember(Name = "replyTo", EmitDefaultValue = false)]
        public SendSmtpEmailRecipient ReplyTo { get; set; }

        /// <summary>
        /// Pass the absolute URL (no local file) or the base64 content of the attachment along with the attachment name (Mandatory if attachment content is passed). For example, [{&#39;url&#39;:&#39;https://attachment.domain.com/myAttachmentFromUrl.jpg&#39;, &#39;name&#39;:&#39;My attachment 1&#39;}, {&#39;content&#39;:&#39;base64 exmaple content&#39;, &#39;name&#39;:&#39;My attachment 2&#39;}]. Allowed extensions for attachment file: xlsx, xls, ods, docx, docm, doc, csv, pdf, txt, gif, jpg, jpeg, png, tif, tiff, rtf, bmp, cgm, css, shtml, html, htm, zip, xml, ppt, pptx, tar, ez, ics, mobi, msg, pub, eps and odt ( If &#39;templateId&#39; is passed and is in New Template Language format then only attachment url is accepted. If template is in Old template Language format, then &#39;attachment&#39; is ignored )
        /// </summary>
        /// <value>Pass the absolute URL (no local file) or the base64 content of the attachment along with the attachment name (Mandatory if attachment content is passed). For example, [{&#39;url&#39;:&#39;https://attachment.domain.com/myAttachmentFromUrl.jpg&#39;, &#39;name&#39;:&#39;My attachment 1&#39;}, {&#39;content&#39;:&#39;base64 exmaple content&#39;, &#39;name&#39;:&#39;My attachment 2&#39;}]. Allowed extensions for attachment file: xlsx, xls, ods, docx, docm, doc, csv, pdf, txt, gif, jpg, jpeg, png, tif, tiff, rtf, bmp, cgm, css, shtml, html, htm, zip, xml, ppt, pptx, tar, ez, ics, mobi, msg, pub, eps and odt ( If &#39;templateId&#39; is passed and is in New Template Language format then only attachment url is accepted. If template is in Old template Language format, then &#39;attachment&#39; is ignored )</value>
        [DataMember(Name = "attachment", EmitDefaultValue = false)]
        public List<SendSmtpEmailAttachment> Attachment { get; set; }

        /// <summary>
        /// Pass the set of headers that shall be sent along the mail headers in the original email. &#39;sender.ip&#39; header can be set (only for dedicated ip users) to mention the IP to be used for sending transactional emails. For example, {&#39;Content-Type&#39;:&#39;text/html&#39;, &#39;charset&#39;:&#39;iso-8859-1&#39;, &#39;sender.ip&#39;:&#39;1.2.3.4&#39;}
        /// </summary>
        /// <value>Pass the set of headers that shall be sent along the mail headers in the original email. &#39;sender.ip&#39; header can be set (only for dedicated ip users) to mention the IP to be used for sending transactional emails. For example, {&#39;Content-Type&#39;:&#39;text/html&#39;, &#39;charset&#39;:&#39;iso-8859-1&#39;, &#39;sender.ip&#39;:&#39;1.2.3.4&#39;}</value>
        [DataMember(Name = "headers", EmitDefaultValue = false)]
        public Object Headers { get; set; }

        /// <summary>
        /// Id of the template
        /// </summary>
        /// <value>Id of the template</value>
        [DataMember(Name = "templateId", EmitDefaultValue = false)]
        public long? TemplateId { get; set; }

        /// <summary>
        /// Pass the set of attributes to customize the template. For example, {&#39;FNAME&#39;:&#39;Joe&#39;, &#39;LNAME&#39;:&#39;Doe&#39;}. It&#39;s considered only if template is in New Template Language format.
        /// </summary>
        /// <value>Pass the set of attributes to customize the template. For example, {&#39;FNAME&#39;:&#39;Joe&#39;, &#39;LNAME&#39;:&#39;Doe&#39;}. It&#39;s considered only if template is in New Template Language format.</value>
        [DataMember(Name = "params", EmitDefaultValue = false)]
        public Object Params { get; set; }

        /// <summary>
        /// Tag your emails to find them more easily
        /// </summary>
        /// <value>Tag your emails to find them more easily</value>
        [DataMember(Name = "tags", EmitDefaultValue = false)]
        public List<string> Tags { get; set; }

    }
}
