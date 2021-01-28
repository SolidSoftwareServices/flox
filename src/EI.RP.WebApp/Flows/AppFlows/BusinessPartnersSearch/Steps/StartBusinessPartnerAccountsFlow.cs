using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Session;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models.Interop;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Mvc.Controllers;
using EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.Steps
{
	public class StartBusinessPartnerAccountsFlow : BusinessPartnersSearchScreen
	{
		private readonly IUserSessionProvider _userSessionProvider;

		public StartBusinessPartnerAccountsFlow(IUserSessionProvider userSessionProvider)
		{
			_userSessionProvider = userSessionProvider;
		}

		public override ScreenName ScreenStep => BusinessPartnersSearchStep.SelectPartnerAndConnectToAccountsFlow;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var screenModel = contextData.GetStepData<SearchAndShowResults.ScreenModel>();
			_userSessionProvider.ActingAsBusinessPartnerId = screenModel.SelectedBusinessPartnerId;
			if (!string.IsNullOrWhiteSpace(screenModel.UserName))
			{
				_userSessionProvider.ActingAsUserName = screenModel.UserName;
			}
            return new StepData(ResidentialPortalFlowType.Accounts.ToString(), nameof(UiFlowController.Init));
        }
    }

    public class StepData : UiFlowExitRedirection
    {
        public StepData(string controllerName, string actionName) : base(controllerName, actionName)
        {

        }

        public override IEnumerable<ScreenEvent> DontValidateEvents => new List<ScreenEvent>()
            {ScreenEvent.ErrorOccurred, ScreenEvent.Start};
    }
}