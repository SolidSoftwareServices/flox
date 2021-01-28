using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.WebApp.Flows.AppFlows.EnergyServicesAccountOverview.FlowDefinitions
{
	public interface IEnergyServicesAccountOverviewInput : IFlowInputContract
	{
		string AccountNumber { get; set; }
	}

    public class EnergyServicesAccountOverviewInput : IEnergyServicesAccountOverviewInput
    {
		public string AccountNumber { get; set; }
	}
}
