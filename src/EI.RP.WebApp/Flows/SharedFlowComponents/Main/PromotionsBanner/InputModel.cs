using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.PromotionsBanner
{
	public class InputModel
	{
		public string AccountNumber { get; set; }
		public ScreenEvent ToPromotionEvent { get; set; }
		public ScreenEvent DismissBannerEvent { get; set; }
	}
}
