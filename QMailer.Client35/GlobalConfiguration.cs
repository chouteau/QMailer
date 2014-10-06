using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QMailer.Client35
{
	public static class GlobalConfiguration
	{
		private static object m_Lock = new object();
		private static QMailerConfiguration m_Config;

		public static QMailerConfiguration Configuration
		{
			get
			{
				if (m_Config == null)
				{
					lock(m_Lock)
					{
						if (m_Config == null)
						{
							m_Config = new QMailerConfiguration();
						}
					}
				}

				return m_Config;
			}
		}
		
	}
}
