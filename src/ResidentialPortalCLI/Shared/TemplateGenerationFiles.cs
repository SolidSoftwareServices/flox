using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NLog;
using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Shared
{
	public partial class TemplateGenerationFiles
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private readonly ConcurrentBag<GeneratedFile> _generatedFiles = new ConcurrentBag<GeneratedFile>();

		public void Clear()
		{
			_generatedFiles.Clear();
		}

		public async Task FlushFiles()
		{
			foreach (var generatedFile in _generatedFiles)
			{
				await generatedFile.Flush();
			}

			Clear();
		}

		public void GenerateFile(ICodeGenerationTemplate template,string toFolder, string asClassName,bool removeIfExists=true,bool isPartial=true)
		{
			if (template.MustGenerate)
			{
				Logger.Info(() => $"Generating {asClassName}...");
				var codeRaw = template.TransformText();
				var codeText = template.Extension.Equals("cs", StringComparison.InvariantCultureIgnoreCase)
					? CSharpSyntaxTree.ParseText(codeRaw)
						.GetRoot()
						.NormalizeWhitespace().ToFullString()
					: codeRaw;

				_generatedFiles.Add(new GeneratedFile(toFolder, isPartial?$"{asClassName}.generated.{template.Extension}": $"{asClassName}.{template.Extension}", codeText,
					template.MustGenerate, removeIfExists));
				
			}
		}

	}
}