using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.WebApp.Flows.AppFlows.EnergyServicesAccountOverview.FlowDefinitions;
using NLog;
using System.Threading.Tasks;
using System.Collections.Generic;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Infrastructure.PresentationServices.EventsPublisher;

namespace EI.RP.WebApp.Flows.AppFlows.EnergyServicesAccountOverview.Steps
{
    public class Main : EnergyServicesAccountOverviewScreen
	{
		private readonly IDomainQueryResolver _domainQueryResolver;
        private readonly IUIEventPublisher _eventsPublisher;

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public Main(IDomainQueryResolver domainQueryResolver,
            IUIEventPublisher eventsPublisher)
		{
			_domainQueryResolver = domainQueryResolver;
            _eventsPublisher = eventsPublisher;
		}

        public override string ViewPath { get; } = "EnergyServicesAccountOverview_Main";
        public override ScreenName ScreenStep => EnergyServicesAccountOverviewStep.EnergyServiceAccountOverviewDefault;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred);
		}

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootStepData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
            var accountNumber = rootStepData.AccountNumber;

            var accountInfo = await _domainQueryResolver.GetAccountInfoByAccountNumber(accountNumber);

            var stepData = new ScreenModel
            {
                AccountNumber = accountNumber,
                Partner = accountInfo.Partner
            };

            await Task.WhenAll(PublishViewRequestedEvent(stepData));

            return stepData;
        }

        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
            UiFlowScreenModel originalScreenModel,
            IDictionary<string, object> stepViewCustomizations = null)
        {
            var refreshedStepData = originalScreenModel.CloneDeep();
            var data = (ScreenModel)refreshedStepData;

            if (stepViewCustomizations != null)
            {
                data.SetFlowCustomizableValue(stepViewCustomizations, x => x.PageIndex);
            }

            return data;
        }

        private Task PublishViewRequestedEvent(ScreenModel screenModel)
        {
            return _eventsPublisher.Publish(new UiEventInfo
            {
                Description = "View Account Dashboard",
                AccountNumber = screenModel.AccountNumber,
                Partner = screenModel.Partner,
                SubCategoryId = 1203,
                CategoryId = 120
			});
        }

		public static class StepEvent
		{
		}

		public class ScreenModel : UiFlowScreenModel
        {
            public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == EnergyServicesAccountOverviewStep.EnergyServiceAccountOverviewDefault;
			}

            public string AccountNumber { get; set; }

            public string Partner { get; set; }

            public int PageIndex { get; set; }
        }
	}
}