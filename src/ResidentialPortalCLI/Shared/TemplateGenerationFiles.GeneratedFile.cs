using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using NLog;

namespace ResidentialPortalCLI.Shared
{
	public partial class TemplateGenerationFiles
	{
		private  class GeneratedFile
		{
			private static ILogger Logger = LogManager.GetCurrentClassLogger();
			private readonly bool _mustGenerate;
			private readonly bool _removeIfExists;
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

			public GeneratedFile(string finalPath, string fileName, string fileContent, bool mustGenerate,bool removeIfExists)
			{
				_mustGenerate = mustGenerate;
				_removeIfExists = removeIfExists;
				FinalPath = Path.Combine(finalPath, fileName);
				FileName = fileName;
				TempPath = Path.Combine(TempGenerated, $"{fileName}{DateTime.UtcNow.Ticks.ToString()}");
				File.WriteAllText(TempPath, fileContent);
			}

			public string FileName { get; set; }


			public string FinalPath { get; }
			public string TempPath { get; }

			public async Task Flush()
			{
				if (File.Exists(FinalPath))
				{
					if (_removeIfExists)
					{
						File.Delete(FinalPath);
						Logger.Info(() => $"Removed existing file {FinalPath}");
					}
					else
					{
						Logger.Info(() => $"File not generated as it already exists {FinalPath}");

						return;
					}
				}

				if (_mustGenerate)
				{
					var directoryName = Path.GetDirectoryName(FinalPath);
					if (!Directory.Exists(directoryName))
					{
						try
						{
							Directory.CreateDirectory(directoryName);
						}
						catch (IOException ex)
						{
							Logger.Warn(()=>ex.ToString());
						}
					}
					var content = await File.ReadAllTextAsync(TempPath);
					await File.WriteAllTextAsync(FinalPath, content);
					Logger.Info(() => $"Generated {FinalPath}");
				}
			}
		}
	}
}