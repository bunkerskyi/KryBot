using System;
using System.Reflection;

namespace KryBot.Core.Helpers
{
	public class ResourceHelper
	{
		public static string GetResourceLookup(Type resourceType, string resourceName)
		{
			if ((resourceType != null) && (resourceName != null))
			{
				var property = resourceType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static);
				if (property == null)
				{
					throw new InvalidOperationException("Resource type does not have property");
				}
				if (property.PropertyType != typeof(string))
				{
					throw new InvalidOperationException("Resource property is not string type");
				}
				return (string)property.GetValue(null, null);
			}
			return null;
		}
	}
}
