using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Flows.CreateFlowComponent.Builders.ViewModelBuilder
{
	partial class ViewModelBuilderTemplate : ICodeGenerationTemplate
	{
		public string Extension => "cs";
		public CreateFlowComponentOptions Options { get; }

		public ViewModelBuilderTemplate(CreateFlowComponentOptions options)
		{
			Options = options;
		}

		public bool MustGenerate => true;
	}
}
