using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;

namespace ResidentialPortalCLI.OData.ODataProxy.Generators
{
	partial class SapProxyGenerator
	{
		private class GeneratedFile
		{
			private readonly bool _mustGenerate;
			private static readonly string TempGenerated;

			static GeneratedFile()
			{
				TempGenerated = Path.Combine(Assembly.GetExecutingAssembly().GetDirectory().FullName, "TempGenerated");
				if (Directory.Exists(TempGenerated))
				{
					Directory.Delete(TempGenerated, true);
				}

				Directory.CreateDirectory(TempGenerated);
			}

			public GeneratedFile(string finalPath, string fileName, string fileContent, bool mustGenerate)
			{
				_mustGenerate = mustGenerate;
				FinalPath = Path.Combine(finalPath, fileName);
				FileName = fileName;
				TempPath = Path.Combine(TempGenerated, $"{fileName}{DateTime.UtcNow.Ticks.ToString()}");
				File.WriteAllText(TempPath, fileContent);
			}

			public string FileName { get; set; }


			public string FinalPath { get; }
			public string TempPath { get; }

			public async Task WriteGeneratedFiles()
			{
				if (File.Exists(FinalPath))
				{
					File.Delete(FinalPath);
					Logger.Info(() => $"Removed existing file {FinalPath}");
				}

				if (_mustGenerate)
				{
					var content = await File.ReadAllTextAsync(TempPath);
					await File.WriteAllTextAsync(FinalPath, content);
					Logger.Info(() => $"Generated {FinalPath}");
				}
			}
		}
	}
}