using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.Plan.Components.AccountBilling
{
	/// <summary>
	/// This class defines the the component AccountBilling input parameter
	/// </summary>
	public class InputModel
	{
		public string AccountNumber { get; set; }

		public ScreenEvent SwitchOffPaperBillEvent { get; set; }
		public ScreenEvent SwitchOnPaperBillEvent { get; set; }

		public ScreenEvent SwitchOnMonthlyBillingEvent { get; set; }
		public ScreenEvent SwitchOffMonthlyBillingEvent { get; set; }

		public ScreenEvent SwitchOffMeterData { get; set; }

		public bool NoAccessToFeatures { get; set; }
		public bool MovedToStandardPlan { get; set; }
		public bool AgreeTermsAndConditions { get; set; }
		public bool IsMoveToStandardPlanRequestSendSuccesfully { get; set; }
	}
}