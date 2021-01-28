using System.Collections.Generic;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.WebApp.Flows.AppFlows.Accounts.FlowDefinitions;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Steps
{
	public class ShowFlowCollectiveAccountError : UiFlowContainerScreen<ResidentialPortalFlowType>
	{
	    public static class StepEvent
	    {
		    public static readonly ScreenEvent ShowFlowCollectiveAccountError = new ScreenEvent(nameof(ShowFlowCollectiveAccountError), nameof(ShowFlowCollectiveAccountError));
	    }
		
	    public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.Accounts;

		public override ScreenName ScreenStep => CustomerAccountsStep.ShowCollectiveAccountError;
	    
		public override string ViewPath => "FlowCollectiveAccountError";
		
		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var screenModel = new ScreenModel();

			await SetTitle(contextData, screenModel);

		    return screenModel;
	    }

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var refreshedStepData = originalScreenModel.CloneDeep();

			await SetTitle(contextData, (ScreenModel)refreshedStepData);

			return refreshedStepData;
		}

		private static async Task SetTitle(IUiFlowContextData contextData, ScreenModel model)
		{
			var last = (await contextData.GetCurrentStepContainedData(ContainedFlowQueryOption.Last))
				?.ScreenTitle;
			var immediate = (await contextData.GetCurrentStepContainedData(ContainedFlowQueryOption.Immediate))
				?.ScreenTitle;

			model.ScreenTitle = last ?? immediate;
		}

		public class ScreenModel : UiFlowScreenModel
	    {
		    public ScreenModel() : base(true)
			{
			}
	    }
    }
}
