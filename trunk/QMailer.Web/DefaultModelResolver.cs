using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.Web
{
	public class DefaultModelResolver : IModelResolver
	{
		public string Resolve(string modelName)
		{
			return modelName;
		}

		public EmailModel Convert(EmailConfig emailConfig)
		{
			if (emailConfig == null
				|| emailConfig.Model == null)
			{
				return null;
			}

			var result = new EmailModel()
			{
				Model = emailConfig.Model
			};

			if (emailConfig.Model != null
				&& emailConfig.Model.GetType().AssemblyQualifiedName != emailConfig.AssemblyQualifiedTypeNameModel)
			{
				var modelType = Type.GetType(emailConfig.AssemblyQualifiedTypeNameModel);
				result.Model = Newtonsoft.Json.JsonConvert.DeserializeObject(emailConfig.Model.ToString(), modelType);
			}

			return result;
		}
	}
}
