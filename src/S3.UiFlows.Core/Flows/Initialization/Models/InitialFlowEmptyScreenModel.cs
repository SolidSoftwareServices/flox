using S3.UiFlows.Core.Flows.Screens;

namespace S3.UiFlows.Core.Flows.Initialization.Models
{
	public class InitialFlowEmptyScreenModel : InitialFlowScreenModel
	{
		public override bool IsValidFor(ScreenName screenStep)
		{
			return screenStep == ScreenName.PreStart;
		}
	}
}