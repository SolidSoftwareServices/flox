using System.Threading.Tasks;

namespace EI.RP.UiFlows.Core.Facade.Metadata
{
	public interface IFlowsMetadataResolver
	{
		Task<AppMetadata> GetMetadata();
	}
}