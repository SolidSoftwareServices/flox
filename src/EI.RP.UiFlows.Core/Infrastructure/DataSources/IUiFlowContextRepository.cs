﻿using System.Threading.Tasks;

namespace EI.RP.UiFlows.Core.Infrastructure.DataSources
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