using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QMailer.Web
{
	internal class CacheEntry
	{
		public CacheEntry()
		{
		}

		public string Key { get; set; }
		public object Value { get; set; }
		public DateTime ExpirationDate { get; set; }
	}
}
