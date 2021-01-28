using System;
using System.Threading.Tasks;
using ResidentialPortalCLI.Shared;

namespace ResidentialPortalCLI.Flows
{
	public abstract class FlowOptions
	{
		protected TemplateGenerationFiles GenerationFiles { get; }
		private readonly bool _flushFiles;

		protected FlowOptions() : this(new TemplateGenerationFiles(), true)
		{
		}

		protected internal FlowOptions(TemplateGenerationFiles generationFiles, bool flushFiles = false)
		{
			GenerationFiles = generationFiles;
			_flushFiles = flushFiles;
		}
		protected async Task Execute(Func<TemplateGenerationFiles, Task> generateFilesFunc)
		{
			await generateFilesFunc(GenerationFiles);
			if (_flushFiles)
			{
				await GenerationFiles.FlushFiles();
			}
		}
	}
}