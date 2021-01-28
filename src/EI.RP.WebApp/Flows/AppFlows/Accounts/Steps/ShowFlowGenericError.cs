using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.Accounts.FlowDefinitions;
using NLog.Fluent;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Steps
{
	public class ShowFlowGenericError : CustomerAccountsScreen
	{
		public static class StepEvent
		{
			public static readonly ScreenEvent DetectedUserHasMultipleAccounts = new ScreenEvent(nameof(ShowFlowGenericError),nameof(DetectedUserHasMultipleAccounts));
			public static readonly ScreenEvent Next = new ScreenEvent(nameof(ShowFlowGenericError), nameof(Next));
		}


		public override ScreenName ScreenStep => CustomerAccountsStep.ShowFlowGenericError;
		public override string ViewPath => "FlowGenericError";
		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			

			var stepData = new ScreenModel();
			stepData.LastError = contextData.LastError;

			return stepData;
		}
		public class ScreenModel : UiFlowScreenModel
		{
			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == CustomerAccountsStep.ShowFlowGenericError;
			}

			public UiFlowContextData.ContextError LastError { get; set; }
		}

	}
}