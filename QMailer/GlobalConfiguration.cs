using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer
{
	public static class GlobalConfiguration
	{
		private static object m_Lock = new object();
		private static Lazy<QMailerConfiguration> m_Configuration
			= new Lazy<QMailerConfiguration>(() =>
			{
				var config = new QMailerConfiguration();
				config.BindFromConfiguration(QMailer.Configuration.ConfigurationSettings.AppSettings);

				config.Logger = new DiagnosticsLogger();
				return config;
			}, true);

		public static QMailerConfiguration Configuration
		{
			get
			{
				return m_Configuration.Value;
			}
		}


	}
}
