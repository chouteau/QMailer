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

		public object Convert(object model, string assemblyQualifiedTypeNameModel)
		{
			if (model == null)
			{
				return null;
			}

			object result = model;
			if (model != null
				&& model.GetType().AssemblyQualifiedName != assemblyQualifiedTypeNameModel)
			{
				var modelType = Type.GetType(assemblyQualifiedTypeNameModel);
				result = Newtonsoft.Json.JsonConvert.DeserializeObject(model.ToString(), modelType);
			}

			return result;
		}
	}
}
