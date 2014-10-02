using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QMailer.Web
{
	public class EmailHelper
	{
		public EmailHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
		{
			ViewContext = viewContext;
			Url = new UrlHelper(viewContext.RequestContext);
			ViewData = new ViewDataDictionary(viewDataContainer.ViewData);
		}

		public ViewDataDictionary ViewData { get; private set; }
		public ViewContext ViewContext { get; set; }
		public UrlHelper Url { get; set; }

		public QMailerConfiguration Settings 
		{
			get
			{
				return GlobalConfiguration.Configuration;
			}
		}
	}

	public class EmailHelper<TModel> : EmailHelper
	{
		public EmailHelper(ViewContext viewContext
			, IViewDataContainer viewDataContainer)
			: base(viewContext, viewDataContainer)
		{
			this.ViewData = new ViewDataDictionary<TModel>(viewDataContainer.ViewData);
		}

		public new ViewDataDictionary<TModel> ViewData { get; private set; }
	}

}
