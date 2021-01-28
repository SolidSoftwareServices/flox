using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Flows.CreateNewFlow.Builders.FlowInitializer
{
	partial class FlowInitializerTemplate : ICodeGenerationTemplate
	{
		public CreateNewFlowOptions Options { get; }

		public FlowInitializerTemplate(CreateNewFlowOptions options)
		{
			Options = options;
		}

		public bool MustGenerate => true;
		public string Extension => "cs";
	}
}
