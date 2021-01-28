using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.Plan.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.Plan.Steps
{
	/// <summary>
	///this is the entry point of the flow (Plan) the initial screen is resolved here and as you have the option of holding all the flow context here or in the steps
	/// </summary>
	public class
		PlanFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType, PlanFlowInitializer.RootScreenModel>
	{
		private readonly IUserSessionProvider _userSessionProvider;
		private readonly IDomainQueryResolver _domainQueryResolver;
		private readonly IDomainCommandDispatcher _domainCommandDispatcher;

		public PlanFlowInitializer(IUserSessionProvider userSessionProvider, IDomainQueryResolver domainQueryResolver,
			IDomainCommandDispatcher domainCommandDispatcher)
		{
			_userSessionProvider = userSessionProvider;
			_domainQueryResolver = domainQueryResolver;
			_domainCommandDispatcher = domainCommandDispatcher;
		}

		public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.Plan;

		public override bool Authorize()
		{
			return !_userSessionProvider.IsAnonymous();
		}

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(
			IScreenFlowConfigurator preStartCfg, UiFlowContextData contextData)
		{
			return preStartCfg
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, PlanStep.ShowFlowGenericError)
				.OnEventNavigatesTo(ScreenEvent.Start, PlanStep.MainScreen);
		}

		protected override async Task<RootScreenModel> OnBuildStartData(UiFlowContextData contextData,
			RootScreenModel data)
		{
			return data;
		}

		public class RootScreenModel : InitialFlowScreenModel, IPlanInput
		{
			public string AccountNumber { get; set; }
		}
	}
}