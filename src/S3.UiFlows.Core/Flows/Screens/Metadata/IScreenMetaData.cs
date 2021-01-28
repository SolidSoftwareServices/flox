using System.Collections.Generic;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Core.Flows.Screens.Metadata
{
	internal interface IScreenMetaData
	{
		string ContainedFlowHandler { get; set; }
		string ContainedFlowStartType { get; set; }
		string ContainedFlowType { get; set; }
		string FlowHandler { get; set; }
		string FlowScreenName { get; set; }
		string ContainerController { get; set; }
		IDictionary<string, object> ContainerProperties { get; }

		UiFlowScreenModel UserData { get; set; }


		bool IsFlowContainer { get; set; }

		bool IsContainedInController();


		bool IsWellFormed(out string reason);
	}
}