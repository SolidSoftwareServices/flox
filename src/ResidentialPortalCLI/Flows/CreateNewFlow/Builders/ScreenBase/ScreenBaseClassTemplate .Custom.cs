using ResidentialPortalCLI.Flows.CreateFlowInput;
using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Flows.CreateNewFlow.Builders.ScreenBase
{
	partial class ScreenBaseClassTemplate : ICodeGenerationTemplate
	{
		public string Extension => "cs";
		public CreateNewFlowOptions Options { get; }

		public ScreenBaseClassTemplate(CreateNewFlowOptions options)
		{
			Options = options;
		}

		public bool MustGenerate => true;
	}
}
