using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Flows.CreateFlowComponent.Builders.Component
{
	partial class ComponentTemplate : ICodeGenerationTemplate
	{
		public string Extension => "cs";
		public CreateFlowComponentOptions Options { get; }

		public ComponentTemplate(CreateFlowComponentOptions options)
		{
			Options = options;
		}

		public bool MustGenerate => true;
	}
}
