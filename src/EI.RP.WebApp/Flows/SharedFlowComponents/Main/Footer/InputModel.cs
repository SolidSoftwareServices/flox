using System.Collections.Generic;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.Footer
{
	public class InputModel
	{
        public IEnumerable<NavigationItem> NavigationItems { get; set; }
        public bool IsAlwaysOnBottom { get; set; } = true;
    }
}
