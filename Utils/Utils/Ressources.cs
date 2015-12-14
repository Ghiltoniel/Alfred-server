using System;
using System.Resources;

namespace Alfred.Utils
{
	public class RessourcesReader
    {
		public static string ReadResourceValue(Type type, string key)
		{
			var resourceValue = string.Empty;
			try
			{
                var rm = new ResourceManager(type);
			    resourceValue = rm.GetString(key);
			}
			catch (Exception)
			{
				resourceValue = "";
			}
			return resourceValue;
		}
	}
}
