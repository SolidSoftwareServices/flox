using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using ResidentialPortalCLI.Flows.CreateFlowInput.Builders.InputClass;
using ResidentialPortalCLI.Flows.CreateFlowInput.Builders.InputContract;
using ResidentialPortalCLI.Shared;

namespace ResidentialPortalCLI.Flows.CreateFlowInput
{
	[Verb("createFlowInput", HelpText = "Generates a new flow")]
	public class CreateFlowInputOptions: FlowOptions
	{
	
		public CreateFlowInputOptions():base()
		{
		}

		internal CreateFlowInputOptions(TemplateGenerationFiles generationFiles,bool flushFiles=false):base(generationFiles,flushFiles)
		{
		}

		[Option('p', "projectDir", Required = false, HelpText = "the path to the target project's folder",
			Default = "./../../../../Ei.RP.WebApp")]
		public string ProjectDir { get; set; }

		[Option('o', "overrideExisting", Required = false, HelpText = "overrides existing if exists",
			Default = false)]
		public bool OverrideExisting { get; set; }
		[Option('n', "flowName", Required = true, HelpText = "flow name")]
		public string FlowName { get; set; }


		
		public static async Task<int> Execute(CreateFlowInputOptions input)
		{
			
			try
			{
				var folder = Path.Combine(input.ProjectDir, $"Flows/AppFlows/{input.FlowName}/FlowDefinitions");

				await input.Execute(async g =>
				{
					g.GenerateFile(new InputTemplate(input), folder, $"I{input.FlowName}Input", input.OverrideExisting, false);
					g.GenerateFile(new InputClassTemplate(input), folder, $"{input.FlowName}Input", input.OverrideExisting, false);

				});
				
				
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return -2;
			}

			return 0;
		}
	}
}

	