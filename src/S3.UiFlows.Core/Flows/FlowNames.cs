using System.Collections.Generic;

namespace S3.UiFlows.Core.Flows
{
	class FlowNames
	{
		public IEnumerable<string> Names { get; }

		public FlowNames(IEnumerable<string> flowNames)
		{
			Names = flowNames;
		}
	}
}