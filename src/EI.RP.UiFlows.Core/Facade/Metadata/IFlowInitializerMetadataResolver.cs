using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.UiFlows.Core.Facade.Metadata
{
	internal interface IFlowInitializerMetadataResolver
	{
		Task<AppMetadata.FlowMetadata.FlowStepMetadata> GetMetadata(IUiFlowInitializationStep initializationStep);
	}
}