using System;
using EI.RP.CoreServices.System;
using Microsoft.AspNetCore.Hosting;

namespace EI.RP.CoreServices.DeliveryPipeline.ManualTesting
{
	internal class ConfigurableTestingItems : IConfigurableTestingItems
	{
        public ConfigurableTestingItems(IHostingEnvironment environment)
        {
        }
        public int SimulateAppWorkloadDelaySeconds { get; set; }
        public bool SimulateConsumptionDataFailure { get; set; } = false;
        public bool CanShowCostToDate { get; set; }
        public EuroMoney CostToDateAmount { get; set; } = EuroMoney.Zero;
        public DateTime CostToDateSince { get; set; } = new DateTime();
    }
}