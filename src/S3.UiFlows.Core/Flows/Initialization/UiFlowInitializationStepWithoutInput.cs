using S3.UiFlows.Core.Flows.Initialization.Models;

namespace S3.UiFlows.Core.Flows.Initialization
{
	/// <summary>
	///     Default flow initializer. Flow does not receive any input
	/// </summary>
	public abstract class
		UiFlowStarter : UiFlowInitializationStep< InitialFlowEmptyScreenModel>
	{
	}
}