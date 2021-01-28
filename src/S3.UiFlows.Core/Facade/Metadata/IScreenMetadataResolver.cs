using S3.UiFlows.Core.Flows.Screens;
using System.Threading.Tasks;

namespace S3.UiFlows.Core.Facade.Metadata
{
	internal interface IScreenMetadataResolver
	{
		Task<AppMetadata.FlowMetadata.FlowStepMetadata> GetMetadata(IUiFlowScreen uiFlowScreen);
		
	}
}