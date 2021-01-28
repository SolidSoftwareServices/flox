using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ResidentialPortalCLI.Flows.CreateFlowInput;
using ResidentialPortalCLI.Flows.CreateNewFlow.Builders;
using ResidentialPortalCLI.Flows.CreateNewFlow.Builders.FlowInitializer;
using ResidentialPortalCLI.Flows.CreateNewFlow.Builders.ScreenBase;
using ResidentialPortalCLI.Flows.CreateNewFlow.Builders.StepsEnum;
using ResidentialPortalCLI.Flows.CreateStep;
using ResidentialPortalCLI.Flows.CreateStep.Builders.FlowEmptyView;
using ResidentialPortalCLI.Shared;

namespace ResidentialPortalCLI.Flows.CreateNewFlow
{
	[Verb("createNewFlow", HelpText = "Generates a new flow")]
	public class CreateNewFlowOptions:FlowOptions
	{
		public CreateNewFlowOptions() : base()
		{
		}

		internal CreateNewFlowOptions(TemplateGenerationFiles generationFiles, bool flushFiles = false) : base(generationFiles, flushFiles)
		{
		}

		[Option('p', "projectDir", Required = false, HelpText = "the path to the target project's folder",
			Default = "./../../../../Ei.RP.WebApp")]
		public string ProjectDir { get; set; }

		[Option('o', "overrideExisting", Required = false, HelpText = "overrides existing flow if exists",
			Default = false)]
		public bool OverrideExisting { get; set; }
		[Option('n', "flowName", Required = true, HelpText = "flow name")]
		public string FlowName { get; set; }
		
		[Option('t', "flowTypesEnumName", Required = true, HelpText = "enum where the flows are defined")]
		public string FlowTypesEnumName { get; set; }



		public CreateFlowInputOptions ToCreateFlowInputOptions()
		{
			return new CreateFlowInputOptions(GenerationFiles, false)
			{
				FlowName = FlowName,
				OverrideExisting = OverrideExisting,
				ProjectDir = ProjectDir
			};
		}
		private CreateNewStepOptions ToCreateNewStepInputOptions(string viewName)
		{
			return new CreateNewStepOptions()
			{
				FlowName = FlowName,
				OverrideExisting = OverrideExisting,
				ProjectDir = ProjectDir,
				StepName = viewName
			};
		}
		public static async Task<int> Execute(CreateNewFlowOptions input)
		{
			try
			{
				var flowDefinitionsFolder = Path.Combine(input.ProjectDir, $"Flows/AppFlows/{input.FlowName}/FlowDefinitions");
				var flowStepsFolder = Path.Combine(input.ProjectDir, $"Flows/AppFlows/{input.FlowName}/Steps");

				await input.Execute(async g =>
				{
					await Task.WhenAll(
						FlowFolderStructureBuilder.Build(input),
						FlowTypeEnumEditor.AddFlowEnumMember(input),
						CreateFlowInputOptions.Execute(input.ToCreateFlowInputOptions())
					);
					g.GenerateFile(new ScreenBaseClassTemplate(input), flowDefinitionsFolder, $"{input.FlowName}Screen", input.OverrideExisting, false);
					g.GenerateFile(new StepsClassTemplate(input), flowDefinitionsFolder, $"{input.FlowName}Step", input.OverrideExisting, false);
					g.GenerateFile(new FlowInitializerTemplate(input), flowStepsFolder, $"{input.FlowName}FlowInitializer", input.OverrideExisting, false);

					
				});

				await Task.WhenAll(
					CreateNewStepOptions.Execute(input.ToCreateNewStepInputOptions("ShowFlowGenericError")),
					CreateNewStepOptions.Execute(input.ToCreateNewStepInputOptions("InitialScreen")));

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

	