using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.SendinBlue.Models
{
    [DataContract]
    public class EmailEventReport 
    {
        public EmailEventReport()
        {
            this.Events = default(List<EmailEventReportEvent>);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailEventReport" /> class.
        /// </summary>
        /// <param name="events">events.</param>
        public EmailEventReport(List<EmailEventReportEvent> events = default(List<EmailEventReportEvent>))
        {
            this.Events = events;
        }

        /// <summary>
        /// Gets or Sets Events
        /// </summary>
        [DataMember(Name = "events", EmitDefaultValue = false)]
        public List<EmailEventReportEvent> Events { get; set; }

       
    }
}
