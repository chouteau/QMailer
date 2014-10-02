using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.Tests
{
	public class FakeView : System.Web.Mvc.IView
	{
		public void Render(System.Web.Mvc.ViewContext viewContext, System.IO.TextWriter writer)
		{
			writer.Write("Fake");
		}
	}
}
