using System.Threading.Tasks;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.UiFlows.Core.Facade.Metadata
{
	internal interface IScreenMetadataResolver
	{
		Task<AppMetadata.FlowMetadata.FlowStepMetadata> GetMetadata(IUiFlowScreen uiFlowScreen);
		
	}
}