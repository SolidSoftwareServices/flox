using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using ResidentialPortalCLI.Flows.CreateFlowInput;
using ResidentialPortalCLI.Flows.CreateStep.Builders.FlowEmptyView;
using ResidentialPortalCLI.Flows.CreateStep.Builders.StepCode;
using ResidentialPortalCLI.Shared;

namespace ResidentialPortalCLI.Flows.CreateStep
{
	[Verb("createNewStep", HelpText = "Generates a new Step in flow")]
	public class CreateNewStepOptions:FlowOptions
	{
		public CreateNewStepOptions() : base()
		{
		}

		internal CreateNewStepOptions(TemplateGenerationFiles generationFiles, bool flushFiles = false) : base(generationFiles, flushFiles)
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


		[Option('s', "stepName", Required = true, HelpText = "step name")]
		public string StepName { get; set; }



		public static async Task<int> Execute(CreateNewStepOptions input)
		{
			try
			{
				var flowStepsFolder = Path.Combine(input.ProjectDir, $"Flows/AppFlows/{input.FlowName}/Steps");
				var flowViewsFolder = Path.Combine(input.ProjectDir, $"Flows/AppFlows/{input.FlowName}/Views");
				ThrowIfDirectoryNotExists(flowStepsFolder);
				ThrowIfDirectoryNotExists(flowViewsFolder);


				await input.Execute(async g =>
				{

					g.GenerateFile(new StepCodeTemplate(input), flowStepsFolder, input.StepName, input.OverrideExisting, false);
					AddView(input.StepName);

					await Task.WhenAll(FlowStepsEnumEditor.AddFlowStepEnumMember(input));


					void AddView(string name)
					{
						g.GenerateFile(new FlowEmptyViewTemplate(input, name), flowViewsFolder, name,
							input.OverrideExisting, false);
					}
				});



			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return -2;
			}

			return 0;


			void ThrowIfDirectoryNotExists(string folder)
			{
				if (!Directory.Exists(folder))
					throw new InvalidOperationException($"The flow needs to exist previously. Could not find {folder}");
			}
		}
	}
}

	