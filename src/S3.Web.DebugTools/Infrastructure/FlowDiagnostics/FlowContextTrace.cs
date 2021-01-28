using System.Collections.Generic;
using System.Linq;
using S3.UiFlows.Core.Infrastructure.DataSources;
using ObjectsComparer;

namespace S3.Web.DebugTools.Infrastructure.FlowDiagnostics
{
	public class FlowContextTrace
	{
		public FlowContextTrace(string scopeId, IEnumerable<Difference> diff, UiFlowContextData initial, UiFlowContextData final)
		{
			ScopeId = scopeId;
			Diff = diff;
			Initial = initial;
			Final = final;
		}

		public string ScopeId { get; }

		public IEnumerable<Difference> Diff { get; }

		public int TotalCount()
		{
			return Diff.Count();
		}

		public UiFlowContextData Initial { get; }
		public UiFlowContextData Final { get; }

		public override string ToString()
		{
			return $"{nameof(ScopeId)}: {ScopeId} - Count: {TotalCount()}";
		}
	}


}