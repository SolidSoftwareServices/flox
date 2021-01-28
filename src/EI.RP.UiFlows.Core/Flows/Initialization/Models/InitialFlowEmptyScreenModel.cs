using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.UiFlows.Core.Flows.Initialization.Models
{
	public class InitialFlowEmptyScreenModel : InitialFlowScreenModel
	{
		public override bool IsValidFor(ScreenName screenStep)
		{
			return screenStep == ScreenName.PreStart;
		}
	}
}