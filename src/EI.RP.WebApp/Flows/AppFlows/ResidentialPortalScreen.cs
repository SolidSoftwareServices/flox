using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;

namespace EI.RP.WebApp.Flows.AppFlows
{
	public abstract	class ResidentialPortalScreen : UiFlowScreen<ResidentialPortalFlowType> 
	{
		public void SetTitle(string title, UiFlowScreenModel screenModel)
		{
			screenModel.ScreenTitle = title;
		}
	}
}
