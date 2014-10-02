using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QMailer.Web
{
	public abstract class EmailWebPage : WebViewPage
	{
		public EmailHelper MailHelper { get; private set; }

		public override void InitHelpers()
		{
			base.InitHelpers();
			MailHelper = new EmailHelper(this.ViewContext, this);
		}
	}

	public abstract class EmailWebPage<TModel> : WebViewPage<TModel>
	{
		public EmailHelper<TModel> MailHelper { get; private set; }

		public override void InitHelpers()
		{
			base.InitHelpers();
			MailHelper = new EmailHelper<TModel>(this.ViewContext, this);
		}
	}
}
