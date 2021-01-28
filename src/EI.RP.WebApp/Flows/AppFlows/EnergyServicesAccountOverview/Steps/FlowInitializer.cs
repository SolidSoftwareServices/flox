using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Session;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.EnergyServicesAccountOverview.FlowDefinitions;
using NLog;

namespace EI.RP.WebApp.Flows.AppFlows.EnergyServicesAccountOverview.Steps
{
	public class FlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType, FlowInitializer.RootScreenModel>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IUserSessionProvider _userSessionProvider;

		public override ResidentialPortalFlowType InitializerOfFlowType =>
			ResidentialPortalFlowType.EnergyServicesAccountOverview;

		public FlowInitializer(IUserSessionProvider userSessionProvider) 
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
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred,
					EnergyServicesAccountOverviewStep.ShowFlowGenericError)
				.OnEventNavigatesTo(ScreenEvent.Start,
					EnergyServicesAccountOverviewStep.EnergyServiceAccountOverviewDefault);
		}


		public class RootScreenModel : InitialFlowScreenModel, IEnergyServicesAccountOverviewInput
		{
			public string AccountNumber { get; set; }
		}
	}
}