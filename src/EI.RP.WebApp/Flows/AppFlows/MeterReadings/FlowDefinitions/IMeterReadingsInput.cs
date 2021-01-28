using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.Steps;

namespace EI.RP.WebApp.Flows.AppFlows.MeterReadings.FlowDefinitions
{
	public interface IMeterReadingsInput : IFlowInputContract
	{
		string AccountNumber { get; set; }
		MeterReadingFlowInitializer.StartType StartType { get; set; }
	}

	public class MeterReadingsInput : IMeterReadingsInput
	{
		public string AccountNumber { get; set; }
		public MeterReadingFlowInitializer.StartType StartType { get; set; }
	}
}