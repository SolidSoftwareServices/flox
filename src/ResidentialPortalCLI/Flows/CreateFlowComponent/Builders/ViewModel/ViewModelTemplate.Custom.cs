using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Flows.CreateFlowComponent.Builders.ViewModel
{
	partial class ViewModelTemplate : ICodeGenerationTemplate
	{
		public string Extension => "cs";
		public CreateFlowComponentOptions Options { get; }

		public ViewModelTemplate(CreateFlowComponentOptions options)
		{
			Options = options;
		}

		public bool MustGenerate => true;
	}
}
