using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EI.RP.CoreServices.EmbeddedResources
{
	public static class EmbeddedResourceReader
	{
		public static string ReadTextResource(string fileName)
		{
			using (var reader = new StreamReader(ReadTextResourceAsStream(fileName)))
			{
				return reader.ReadToEnd();
			}
		}

		public static Stream ReadTextResourceAsStream(string fileName)
		{

			var assembly = Assembly.GetCallingAssembly();

			var resourcePath = assembly.GetManifestResourceNames()
				.Single(str => str.EndsWith(fileName));

			return assembly.GetManifestResourceStream(resourcePath);
		}
	}
}
