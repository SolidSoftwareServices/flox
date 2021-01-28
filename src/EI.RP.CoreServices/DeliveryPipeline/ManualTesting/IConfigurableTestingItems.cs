using System;
using EI.RP.CoreServices.System;

namespace EI.RP.CoreServices.DeliveryPipeline.ManualTesting
{
	public interface IConfigurableTestingItems
	{
		bool CanShowCostToDate { get; set; }
        EuroMoney CostToDateAmount { get; set; }
        DateTime CostToDateSince { get; set; }
        int SimulateAppWorkloadDelaySeconds { get; set; }
        bool SimulateConsumptionDataFailure { get; set; }
	}
}