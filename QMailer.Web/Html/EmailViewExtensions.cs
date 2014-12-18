using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QMailer.Web.Html
{
	public static class EmailViewExtensions
	{
		/// <summary>
		/// Ajout d'un header
		/// </summary>
		/// <param name="helper">The helper.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static IHtmlString AddEmailHeader(this HtmlHelper helper, string key, string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return null;
			}
			var emaillist = value.Replace(",", ";").Split(';');
			string append = null;
			foreach (var email in emaillist)
			{
				append = append + string.Format("{0}: {1}\r\n", key, email);
			}
			return helper.Raw(append);
		}

		public static IHtmlString AddEmailHeader(this HtmlHelper helper, string key, string email, string fullName)
		{
			return helper.Raw(string.Format("{0}: {1} <{2}>", key, fullName, email));
		}

		public static IHtmlString AddAttachment(this HtmlHelper helper, string attachmentName, string contentType, string fileName)
		{
			var header = string.Format("attachment:{1}|{2}|{3}", attachmentName, contentType, fileName);
			return helper.Raw(header);
		}

	}
}
