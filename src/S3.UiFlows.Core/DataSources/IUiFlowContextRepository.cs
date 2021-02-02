using System.Threading.Tasks;

namespace S3.UiFlows.Core.DataSources
{
	public interface IUiFlowContextRepository
	{
		UiFlowContextData CreateNew();
		Task<UiFlowContextData> Get(string flowHandler);

		Task<UiFlowContextData> GetRootContainerContext(UiFlowContextData ctx);
		Task<UiFlowContextData> GetRootContainerContext(string flowHandler);
		Task<UiFlowContextData> GetCurrentSnapshot(string flowHandler);
		Task<UiFlowContextData> CreateSnapshotOfContext(UiFlowContextData source);
		Task Flush();
		Task<UiFlowContextData> GetLastSnapshot();
	}
}