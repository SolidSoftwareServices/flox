using ResidentialPortalCLI.Flows.CreateNewFlow;
using ResidentialPortalCLI.OData.ODataProxy.Generators;

namespace ResidentialPortalCLI.Flows.CreateStep.Builders.FlowEmptyView
{
	partial class FlowEmptyViewTemplate : ICodeGenerationTemplate
	{
		public string ViewName { get; }
		public CreateNewStepOptions Options { get; }

		public FlowEmptyViewTemplate(CreateNewStepOptions options,string viewName)
		{
			ViewName = viewName;
			Options = options;
		}

		public bool MustGenerate => true; public string Extension => "cshtml";
	}
}
