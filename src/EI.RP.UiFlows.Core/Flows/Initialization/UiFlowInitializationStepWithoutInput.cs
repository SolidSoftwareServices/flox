using EI.RP.UiFlows.Core.Flows.Initialization.Models;

namespace EI.RP.UiFlows.Core.Flows.Initialization
{
	/// <summary>
	///     Default flow initializer. Flow does not receive any input
	/// </summary>
	/// <typeparam name="TFlowType"></typeparam>
	public abstract class
		UiFlowInitializationStep<TFlowType> : UiFlowInitializationStep<TFlowType, InitialFlowEmptyScreenModel>
	{
	}
}