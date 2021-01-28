using System.Threading.Tasks;
using S3.UiFlows.Core.Flows.Initialization;

namespace S3.UiFlows.Core.Facade.Metadata
{
	internal interface IFlowInitializerMetadataResolver
	{
		Task<AppMetadata.FlowMetadata.FlowStepMetadata> GetMetadata(IUiFlowInitializationStep initializationStep);
	}
}