using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions
{
	public interface IMovingHouseInput : IFlowInputContract
	{
		string InitiatedFromAccountNumber { get; set; }
    }

	public class MovingHouseInput : IMovingHouseInput
	{
		public string InitiatedFromAccountNumber { get; set; }
	}
}