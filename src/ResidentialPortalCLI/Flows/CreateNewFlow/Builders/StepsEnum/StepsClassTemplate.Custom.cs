using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Flows.CreateNewFlow.Builders.StepsEnum
{
	partial class StepsClassTemplate : ICodeGenerationTemplate
	{
		public string Extension => "cs";
		public CreateNewFlowOptions Options { get; }

		public StepsClassTemplate(CreateNewFlowOptions options)
		{
			Options = options;
		}

		public bool MustGenerate => true;
	}
}
