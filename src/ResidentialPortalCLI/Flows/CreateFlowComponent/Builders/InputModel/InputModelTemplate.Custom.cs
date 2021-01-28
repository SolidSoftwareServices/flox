using ResidentialPortalCLI.Flows.CreateFlowInput;
using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Flows.CreateFlowComponent.Builders.InputModel
{
	partial class InputModelTemplate : ICodeGenerationTemplate
	{
		public string Extension => "cs";
		public CreateFlowComponentOptions Options { get; }

		public InputModelTemplate(CreateFlowComponentOptions options)
		{
			Options = options;
		}

		public bool MustGenerate => true;
	}
}
