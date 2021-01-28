using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Flows.CreateFlowComponent.Builders.View
{
	partial class ViewTemplate : ICodeGenerationTemplate
	{
		public string Extension => "cshtml";
		public CreateFlowComponentOptions Options { get; }

		public ViewTemplate(CreateFlowComponentOptions options)
		{
			Options = options;
		}

		public bool MustGenerate => true;
	}
}
