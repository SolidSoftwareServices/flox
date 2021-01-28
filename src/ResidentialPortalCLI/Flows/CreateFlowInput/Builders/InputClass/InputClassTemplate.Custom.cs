using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Flows.CreateFlowInput.Builders.InputClass
{
	partial class InputClassTemplate : ICodeGenerationTemplate
	{
		public string Extension => "cs";
		public CreateFlowInputOptions Options { get; }

		public InputClassTemplate(CreateFlowInputOptions options)
		{
			Options = options;
		}

		public bool MustGenerate => true;
	}
}
