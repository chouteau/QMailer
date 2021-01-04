using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer
{
	class EmailerServiceFactory : IEmailerServiceFactory
	{
		public IEmailerService Create()
		{
			return new EmailerService();
		}
	}
}
