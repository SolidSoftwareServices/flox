using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using ResidentialPortalCLI.Flows.CreateFlowComponent.Builders.Component;
using ResidentialPortalCLI.Flows.CreateFlowComponent.Builders.InputModel;
using ResidentialPortalCLI.Flows.CreateFlowComponent.Builders.View;
using ResidentialPortalCLI.Flows.CreateFlowComponent.Builders.ViewModel;
using ResidentialPortalCLI.Flows.CreateFlowComponent.Builders.ViewModelBuilder;
using ResidentialPortalCLI.Flows.CreateFlowComponent.Builders.ViewModelBuilderUnitTests;
using ResidentialPortalCLI.Shared;

namespace ResidentialPortalCLI.Flows.CreateFlowComponent
{
	[Verb("createFlowComponent", HelpText = "Generates a new flow component")]
	public class CreateFlowComponentOptions: FlowOptions
	{
	
		public CreateFlowComponentOptions():base()
		{
		}

		internal CreateFlowComponentOptions(TemplateGenerationFiles generationFiles,bool flushFiles=false):base(generationFiles,flushFiles)
		{

		}

		[Option('d', "projectDir", Required = false, HelpText = "the path to the target project's folder",
			Default = "./../../../../Ei.RP.WebApp")]
		public string ProjectDir { get; set; }

		[Option('o', "overrideExisting", Required = false, HelpText = "overrides existing if exists", Default = false)]
		public bool OverrideExisting { get; set; }

		[Option('s', "isShared", Required = false, HelpText = "indicates if the component is shared between 2 or more flows", Default = false)]
		public bool IsShared { get; set; }

		[Option('f', "flowName", Required = false, HelpText = "if is not shared specifies the flow hosting the component")]
		public string FlowName { get; set; }

		[Option('c', "componentName", Required = true, HelpText = "the name of the component")]
		public string ComponentName { get; set; }

		[Option('p', "isPageable", Required = false, HelpText = "indicates if the component is shows paged data", Default = false)]
		public bool IsPageable { get; set; }

		
		public string Namespace()
		{
			
			return IsShared
				? $"{RootNamespace()}.Flows.SharedFlowComponents.Main.{ComponentName}"
				: $"{RootNamespace()}.Flows.AppFlows.{FlowName}.Components.{ComponentName}";
		}

		public string TestsNamespace()
		{
			return IsShared
				? $"{RootNamespace()}.UnitTests.Flows.SharedFlowComponents.Main.{ComponentName}"
				: $"{RootNamespace()}.UnitTests.Flows.AppFlows.{FlowName}.Components.{ComponentName}";
		}

		private string RootNamespace()
		{
			return ProjectDir.Split('/').Last(x => !string.IsNullOrWhiteSpace(x));
		}
		public static async Task<int> Execute(CreateFlowComponentOptions input)
		{
			
			try
			{
				if(!input.IsShared && string.IsNullOrWhiteSpace(input.FlowName)) throw new ArgumentException("The flow name must be specified");

				var folder = input.IsShared
					? Path.Combine(input.ProjectDir, $"Flows/AppFlows/SharedFlowComponents/Main/{input.ComponentName}")
					: Path.Combine(input.ProjectDir, $"Flows/AppFlows/{input.FlowName}/Components/{input.ComponentName}");
				var unitTestsFolder = input.IsShared
					? Path.Combine($"{input.ProjectDir}.UnitTests", $"Flows/AppFlows/SharedFlowComponents/Main/{input.ComponentName}")
					: Path.Combine($"{input.ProjectDir}.UnitTests", $"Flows/AppFlows/{input.FlowName}/Components/{input.ComponentName}");
				await input.Execute( g =>
				{
					g.GenerateFile(new InputModelTemplate(input), folder, $"InputModel", input.OverrideExisting, false);
					g.GenerateFile(new ComponentTemplate(input), folder, $"Component", input.OverrideExisting, false);
					g.GenerateFile(new ViewTemplate(input), folder, input.ComponentName, input.OverrideExisting, false);
					g.GenerateFile(new ViewModelTemplate(input), folder, $"ViewModel", input.OverrideExisting, false);
					g.GenerateFile(new ViewModelBuilderTemplate(input), folder, $"ViewModelBuilder", input.OverrideExisting, false);
					g.GenerateFile(new ViewModelBuilderUnitTestsTemplate(input), unitTestsFolder, $"{input.ComponentName}_ViewModelBuilder_Tests", input.OverrideExisting, false);
					return Task.CompletedTask;
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

	