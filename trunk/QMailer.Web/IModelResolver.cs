using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMailer.Web
{
	public interface IModelResolver
	{
		string Resolve(string modelName);

		object Convert(object model, string assemblyQualifiedTypeNameModel);
	}
}
