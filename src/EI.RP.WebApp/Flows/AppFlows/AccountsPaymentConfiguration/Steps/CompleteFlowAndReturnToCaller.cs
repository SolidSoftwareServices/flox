using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models.Interop;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps
{
	public class CompleteFlowAndReturnToCaller : AccountsPaymentConfigurationScreen
	{
		public override ScreenName ScreenStep => AccountsPaymentConfigurationStep.CompleteFlowAndReturnToCaller;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var initData = contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart);

			if (contextData.LastError != null)
			{
				//error should be logged here
				initData.FlowResult.Exit = AccountsPaymentConfigurationResult.ExitType.ErrorOcurred;
				
				//error handled
				contextData.LastError = null;
			}

			return new CallbackOriginalFlow(initData, initData.FlowResult);
		}
	}
}