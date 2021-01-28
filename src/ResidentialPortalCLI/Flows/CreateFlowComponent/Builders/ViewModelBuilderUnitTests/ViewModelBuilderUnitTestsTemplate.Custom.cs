using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Flows.CreateFlowComponent.Builders.ViewModelBuilderUnitTests
{
	partial class ViewModelBuilderUnitTestsTemplate : ICodeGenerationTemplate
	{
		public string Extension => "cs";
		public CreateFlowComponentOptions Options { get; }

		public ViewModelBuilderUnitTestsTemplate(CreateFlowComponentOptions options)
		{
			Options = options;
		}

		public bool MustGenerate => true;
	}
}
