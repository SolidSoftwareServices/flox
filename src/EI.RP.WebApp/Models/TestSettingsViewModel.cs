using System;
using EI.RP.WebApp.Models.Shared;

namespace EI.RP.WebApp.Models
{
    public class TestSettingsViewModel : LayoutWithNavModel
    {
	    public bool CanShowCostToDate { get; set; }
        public decimal CostToDateAmount { get; set; }
        public DateTime CostToDateSince { get; set; }
        public int SimulateAppWorkloadDelaySeconds { get; set; }
        public bool SimulateConsumptionDataFailure { get; set; }
	}
}