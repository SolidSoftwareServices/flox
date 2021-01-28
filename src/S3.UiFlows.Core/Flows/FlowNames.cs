using System.Collections.Generic;

namespace S3.UiFlows.Core.Infrastructure.IoC
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