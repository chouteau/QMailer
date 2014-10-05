using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QMailer
{
	public static class Extensions
	{
		public static TService GetService<TService>(this IDependencyResolver resolver)
		{
			var result = (TService)resolver.GetService(typeof(TService));
			return result;
		}

		public static IEnumerable<TService> GetServices<TService>(this IDependencyResolver resolver)
		{
			return resolver.GetServices(typeof(TService)).Cast<TService>();
		}

		public static bool IsDynamicPropertyExists(this System.Dynamic.ExpandoObject obj, string propertyName)
		{
			if (obj == null)
			{
				return false;
			}
			return ((IDictionary<String, object>)obj).ContainsKey(propertyName);
		}

		public static void BindFromConfiguration(this object model, System.Collections.Specialized.NameValueCollection nvc)
		{
			if (model == null
				|| nvc == null)
			{
				return;
			}

			var propertyInfoList = model.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty);
			foreach (var propertyInfo in propertyInfoList)
			{
				if (!propertyInfo.CanWrite)
				{
					continue;
				}

				var key = nvc.AllKeys.SingleOrDefault(i => i.Equals(propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase));
				if (key == null)
				{
					continue;
				}

				var value = nvc[key];
				if (value == null)
				{
					continue;
				}

				var typedValue = System.Convert.ChangeType(value, propertyInfo.PropertyType);
				propertyInfo.SetValue(model, typedValue, null);
			}
		}

	}
}
