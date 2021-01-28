using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Infrastructure.Flows;
using System.Collections.Generic;

namespace EI.RP.WebApp.Flows.AppFlows.Agent.Components.AgentFooter
{
    public class ViewModel : FlowComponentViewModel
    {
        public IEnumerable<NavigationItem> NavigationItems { get; set; }
    }
}
