using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Flows.CreateFlowInput.Builders.InputContract
{
	partial class InputTemplate : ICodeGenerationTemplate
	{
		public string Extension => "cs";
		public CreateFlowInputOptions Options { get; }

		public InputTemplate(CreateFlowInputOptions options)
		{
			Options = options;
		}

		public bool MustGenerate => true;
	}
}
