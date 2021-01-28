using EI.RP.CoreServices.Http.Session;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AddGasAccount.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.AddGasAccount.Steps
{
	public class AddGasAccountFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType,
		AddGasAccountFlowInitializer.RootScreenModel>
	{
		private readonly IUserSessionProvider _userSessionProvider;
		public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.AddGasAccount;
		public AddGasAccountFlowInitializer(IUserSessionProvider userSessionProvider) 

		{
			_userSessionProvider = userSessionProvider;
		}

		public override bool Authorize()
		{
			return !_userSessionProvider.IsAnonymous();
		}

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(
			IScreenFlowConfigurator preStartCfg, UiFlowContextData contextData)
		{
			return preStartCfg
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenEvent.Start, AddGasAccountStep.CollectAccountConsumptionDetails);
		}

		public static class StepEvent
		{
		}


		public class RootScreenModel : InitialFlowScreenModel, IAddGasAccountFlowInput
		{
			public string ElectricityAccountNumber { get; set; }
		}
	}
}