using System.IO;
using System.Linq;
using System.Reflection;

namespace S3.CoreServices.EmbeddedResources
{
	public static class EmbeddedResourceReader
	{
		public static string ReadTextResource(string fileName,Assembly assembly=null)
		{
			var ass = assembly??Assembly.GetCallingAssembly();
			using (var reader = new StreamReader(ResolveManifestResourceStream(ass, fileName)))
			{
				return reader.ReadToEnd();
			}
		}

		public static Stream ReadTextResourceAsStream(string fileName,Assembly assembly=null)
		{
			var ass = assembly??Assembly.GetCallingAssembly();

			return ResolveManifestResourceStream(ass, fileName);
		}

		private static Stream ResolveManifestResourceStream(Assembly assembly,string fileName)
		{
			var resourcePath = assembly.GetManifestResourceNames()
				.Single(str => str.EndsWith(fileName));

			return assembly.GetManifestResourceStream(resourcePath);
		}
	}
}
