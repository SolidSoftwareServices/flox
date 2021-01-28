using System;

namespace EI.RP.WebApp.Flows.AppFlows.Usage.Components.UsageChart
{
	public class InputModel
	{
		public string AccountNumber { get; set; }
        public DateTime? MinDate { get; set; }
		public DateTime? MaxDate { get; set; }
		public bool CanCompare { get; set; }
    }
}
