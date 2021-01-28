using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountDashboard.FlowDefinitions;
using NLog;

namespace EI.RP.WebApp.Flows.AppFlows.AccountDashboard.Steps
{
	internal class AccountDashboardFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType,
		AccountDashboardFlowInitializer.RootScreenModel>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IDomainQueryResolver _domainQueryResolver;

		private readonly IUserSessionProvider _userSessionProvider;

		public AccountDashboardFlowInitializer(IUserSessionProvider userSessionProvider,
			IDomainQueryResolver domainQueryResolver)
		{
			_userSessionProvider = userSessionProvider;
			_domainQueryResolver = domainQueryResolver;
		}

		public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.AccountDashboard;

		public override bool Authorize()
		{
			return !_userSessionProvider.IsAnonymous();
		}

		public override IScreenFlowConfigurator
			OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
				UiFlowContextData contextData)
		{
			return preStartCfg
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, AccountDashboardScreenStep.ShowFlowGenericError)
				.OnEventNavigatesTo(ScreenEvent.Start, AccountDashboardScreenStep.MyAccountDashboard);
		}


		protected override async Task<RootScreenModel> OnBuildStartData(UiFlowContextData newContext,
			RootScreenModel data)
		{
			if (data.AccountNumber == null) throw new InvalidOperationException("Account not specified");

			data.AccountType = (await _domainQueryResolver.GetAccountInfoByAccountNumber(data.AccountNumber))
				.ClientAccountType;

			ResolveDefaultFlow();
			return data;

			void ResolveDefaultFlow()
			{
				if (data.InitialFlow == ResidentialPortalFlowType.NoFlow)
					data.InitialFlow = data.AccountType == ClientAccountType.EnergyService
						? ResidentialPortalFlowType.EnergyServicesAccountOverview
						: ResidentialPortalFlowType.Usage;
			}
		}


		protected override async Task<ScreenEvent> OnResolveInitializationEventToTrigger(
			ScreenEvent defaultEventToTriggerAfter,
			UiFlowScreenModel screenModel)
		{
			return ScreenEvent.Start;
		}

		public static class StepEvent
		{
			public static readonly ScreenEvent Next = new ScreenEvent(nameof(AccountDashboardFlowInitializer),
				nameof(Next));
		}


		public class RootScreenModel : InitialFlowScreenModel, IAccountDashboardFlowInput
		{
			public ClientAccountType AccountType { get; set; }
			public string AccountNumber { get; set; }
			public ResidentialPortalFlowType InitialFlow { get; set; }
			public string InitialFlowStartType { get; set; }
			public ResidentialPortalFlowType SourceFlowType { get; set; }
		}
	}
}