using ResidentialPortalCLI.Flows.CreateNewFlow;
using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Flows.CreateStep.Builders.StepCode
{
	partial class StepCodeTemplate : ICodeGenerationTemplate
	{
		public CreateNewStepOptions Options { get; }

		public StepCodeTemplate(CreateNewStepOptions options)
		{
			Options = options;
		}

		public bool MustGenerate => true;
		public string Extension => "cs";
	}
}
