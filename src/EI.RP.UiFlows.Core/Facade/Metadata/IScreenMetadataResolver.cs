using EI.RP.UiFlows.Core.Flows.Screens;
using System.Threading.Tasks;

namespace EI.RP.UiFlows.Core.Facade.Metadata
{
	internal interface IScreenMetadataResolver
	{
		Task<AppMetadata.FlowMetadata.FlowStepMetadata> GetMetadata(IUiFlowScreen uiFlowScreen);
		
	}
}