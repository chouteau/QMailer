using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QMailer.SendInBlue.WebHook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendInBlueController : ControllerBase
    {
		[Route("ping")]
		[HttpGet]
		public object Ping()
		{
			return new
			{
				Date = DateTime.Now
			};
		}

		[Route("event")]
		[HttpPost]
		[HttpGet]
		[HttpPut]
		public void Event(SendinBlue.Models.SendInBlueEvent @event)
		{
			Console.WriteLine(@event);
		}


		[Route("rawevent")]
		[HttpPost]
		[HttpGet]
		[HttpPut]
		public void RawEvent()
		{
			using(var reader = new System.IO.StreamReader(Request.Body, System.Text.Encoding.UTF8))
			{
				var content = reader.ReadToEnd();
				Console.WriteLine(content);
			}
		}
    }
}