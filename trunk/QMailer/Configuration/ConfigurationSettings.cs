using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.Configuration
{
	public static class ConfigurationSettings
	{
		public const string SECTION_NAME = "qmailer";

		private static NameValueCollection m_AppSettings = null;
		public static NameValueCollection AppSettings
		{
			get
			{
				if (m_AppSettings == null)
				{
					m_AppSettings = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection(SECTION_NAME);
					if (m_AppSettings == null)
					{
						m_AppSettings = new NameValueCollection();
					}
				}
				return m_AppSettings;
			}
		}

	}
}
