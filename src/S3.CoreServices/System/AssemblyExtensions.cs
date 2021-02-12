using System;
using System.IO;
using System.Reflection;

namespace S3.CoreServices.System
{
	public static class AssemblyExtensions
	{

		public static string ReadEmbeddedResourceContent(this Assembly assembly,string name)
		{

			using (var stream = assembly.GetManifestResourceStream(name))
			{
				using (var reader = new StreamReader(stream))
				{
					return  reader.ReadToEnd();
				}
			}
		}

		public static DirectoryInfo GetDirectory(this Assembly assembly)
		{
			var location = new Uri(assembly.GetName().CodeBase);
			return new FileInfo(location.AbsolutePath).Directory;
		}
	}
}
