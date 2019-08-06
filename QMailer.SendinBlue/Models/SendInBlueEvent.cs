using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.SendinBlue.Models
{
	/// <summary>
	/// {
	///		"event":"request",
	///		"email":"serge@societe-crea.com",
	///		"id":141893,
	///		"date":"2019-08-05 10:12:50",
	///		"ts":1564992770,
	///		"message-id":"<201908051012.89814935489@smtp-relay.mailin.fr>",
	///		"ts_event":1564992770,
	///		"subject":"Cmd N°CMD0474930 (62.25 € via etransactionpaybox)",
	///		"sending_ip":"77.32.135.142",
	///		"ts_epoch":1564992770000
	///	}
	/// </summary>
	public class SendInBlueEvent
	{
		[JsonProperty(PropertyName = "event")]
		public string EventName { get; set; }

		[JsonProperty(PropertyName = "email")]
		public string Email { get; set; }

		[JsonProperty(PropertyName = "date")]
		public DateTime Date { get; set; }

		[JsonProperty(PropertyName = "message-id")]
		public string MessageId { get; set; }

		[JsonProperty(PropertyName = "subject")]
		public string Subject { get; set; }

		[JsonProperty(PropertyName = "sending_ip")]
		public string SendintIP { get; set; }
	}
}
