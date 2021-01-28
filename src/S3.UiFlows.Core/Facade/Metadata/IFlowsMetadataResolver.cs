using System.Threading.Tasks;

namespace S3.UiFlows.Core.Facade.Metadata
{
	public interface IFlowsMetadataResolver
	{
		Task<AppMetadata> GetMetadata();
	}
}