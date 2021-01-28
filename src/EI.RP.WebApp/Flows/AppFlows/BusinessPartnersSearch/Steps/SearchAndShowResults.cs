using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.Paging;
using Ei.Rp.DomainModels.Contracts;

using EI.RP.DomainServices.Commands.BusinessPartner.Deregister;
using EI.RP.DomainServices.Queries.Contracts.BusinessPartners;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.ErrorHandling;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.Steps
{
    public class SearchAndShowResults : BusinessPartnersSearchScreen
    {

        public override string ViewPath { get; } = "Search";

        private readonly IDomainCommandDispatcher _domainCommandDispatcher;
        private readonly IDomainQueryResolver _queryResolver;
		public SearchAndShowResults(IDomainCommandDispatcher domainCommandDispatcher, IDomainQueryResolver queryResolver)
		{
			_domainCommandDispatcher = domainCommandDispatcher;
			_queryResolver = queryResolver;
		}

        public static class StepEvent
        {
            public static readonly ScreenEvent FetchBusinessPartnersRequested = new ScreenEvent(nameof(SearchAndShowResults),nameof(FetchBusinessPartnersRequested));
            public static readonly ScreenEvent BusinessPartnerSelected = new ScreenEvent(nameof(SearchAndShowResults), nameof(BusinessPartnerSelected));
            public static readonly ScreenEvent DeRegistrationRequested = new ScreenEvent(nameof(SearchAndShowResults), nameof(DeRegistrationRequested));

        }

        public override ScreenName ScreenStep => BusinessPartnersSearchStep.SearchAndShowResultsStep;

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
            IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
                .OnEventReentriesCurrent(StepEvent.FetchBusinessPartnersRequested)
                .OnEventNavigatesTo(StepEvent.BusinessPartnerSelected, BusinessPartnersSearchStep.SelectPartnerAndConnectToAccountsFlow)
                .OnEventReentriesCurrent(StepEvent.DeRegistrationRequested);
        }

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var stepData = new ScreenModel
            {
                MaxRecords = 30
            };
            return stepData;
        }

        protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
            IUiFlowContextData contextData)
        {
            var input = contextData.GetCurrentStepData<ScreenModel>();
            input.ShowSuccessfulMessage = string.Empty;
            if (triggeredEvent == StepEvent.DeRegistrationRequested ||
                triggeredEvent == StepEvent.FetchBusinessPartnersRequested)
            {
	            input.ShowErrorMessage = string.Empty;
				
	            var businessPartnersTask = _queryResolver.GetBusinessPartners(input.MaxRecords,input.UserName,input.LastName,input.City,input.HouseNum,input.PartnerNum, input.Street);
	            if (triggeredEvent == StepEvent.DeRegistrationRequested)
	            {
		            await _domainCommandDispatcher.ExecuteAsync(
			            new DeRegisterBusinessPartnerCommand(input.SelectedBusinessPartnerId, (await businessPartnersTask).Count() == 1));
		            input.ShowSuccessfulMessage = "De-registration process has been completed successfully";
		            input.BusinessPartnerNumbers = new string[0];

				}
	            else if (triggeredEvent == StepEvent.FetchBusinessPartnersRequested)
	            {
		            var businessPartners = (await businessPartnersTask).ToList();
		            if (!businessPartners.Any())
		            {
			            input.ShowErrorMessage = "No customers found for the search criteria entered. Use different search criteria, and try again";
		            }
		            else if (businessPartners.Count > input.MaxRecords)
		            {
			            input.ShowErrorMessage = $"The system found more than {input.MaxRecords} customers for the search criteria entered. Use more restricted search criteria, and try again";
			            businessPartners = businessPartners.Take(input.MaxRecords).ToList();
		            }
		            input.BusinessPartnerNumbers = businessPartners.Select(x=>x.NumPartner).ToArray();
	            }
            }
        }

        protected override bool OnValidateTransitionAttempt(ScreenEvent triggeredEvent, IUiFlowContextData contextData,
            out string errorMessage)
        {
            if (triggeredEvent == StepEvent.FetchBusinessPartnersRequested)
            {
                var stepData = contextData.GetCurrentStepData<ScreenModel>();
                if (!stepData.HasQuery())
                {
                    errorMessage = "Please enter any of the detail to search!";
                    return false;
                }
            }

            return base.OnValidateTransitionAttempt(triggeredEvent, contextData, out errorMessage);
        }

        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData, UiFlowScreenModel originalScreenModel,
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

        public class ScreenModel : UiFlowScreenModel
        {

            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string LastName { get; set; }

            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string UserName { get; set; }

            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string HouseNum { get; set; }

            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string Street { get; set; }
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string City { get; set; }
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string PartnerNum { get; set; }

            [Range(PageSize, 100)]
            public int MaxRecords { get; set; } = 30;


            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == BusinessPartnersSearchStep.SearchAndShowResultsStep;
            }
            public bool HasQuery()
            { 
                return
                    !string.IsNullOrWhiteSpace(LastName)
                    || !string.IsNullOrWhiteSpace(UserName)
                    || !string.IsNullOrWhiteSpace(HouseNum)
                    || !string.IsNullOrWhiteSpace(Street)
                    || !string.IsNullOrWhiteSpace(City)
                    || !string.IsNullOrWhiteSpace(PartnerNum);
            }


            //TODO:??
            public const int PageSize = 12;


            public string ShowSuccessfulMessage { get; set; }
            public string ShowErrorMessage { get; set; }
            public string SelectedBusinessPartnerId { get; set; }

            public IEnumerable<string> BusinessPartnerNumbers { get; set; }
            public int PageIndex { get; set; }
        }
    }
}