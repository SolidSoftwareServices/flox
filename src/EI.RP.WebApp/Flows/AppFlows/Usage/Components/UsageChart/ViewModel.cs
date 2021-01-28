using System;
using System.Collections;
using System.Collections.Generic;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.Usage.Components.UsageChart
{
	public class ViewModel : FlowComponentViewModel
	{
		public string AccountNumber { get; set; }
        public bool CanCompare { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public bool IsSmart { get; set; }
        public bool HasSmartData { get; set; }
        public bool HasNonSmartData { get; set; }
        public IEnumerable<string> StartDatesOfSmartPlan { get; set; }
        public IEnumerable<string> EndDatesOfSmartPlan { get; set; }
        public IEnumerable<string> StartDatesOfNonSmartPlan { get; set; }
    }
}
