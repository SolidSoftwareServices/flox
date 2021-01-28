using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Membership;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows;


using EI.RP.WebApp.Flows.AppFlows.Accounts.FlowDefinitions;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Infrastructure;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Commands.BusinessPartner.ActivateAsEBiller;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Steps
{
    internal class FlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType, FlowInitializer.RootScreenModel>
    {
	    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		public static class StepEvent
        {
            public static readonly ScreenEvent DetectedUserHasMultipleAccounts = new ScreenEvent(nameof(FlowInitializer),nameof(DetectedUserHasMultipleAccounts));
            public static readonly ScreenEvent NoAccountDetected = new ScreenEvent(nameof(FlowInitializer),nameof(NoAccountDetected));
            public static readonly ScreenEvent Next = new ScreenEvent(nameof(FlowInitializer), nameof(Next));
        }

        private readonly IUserSessionProvider _userSessionProvider;
        private readonly IDomainQueryResolver _domainQueryResolver;
		private readonly IDomainCommandDispatcher _domainCommandDispatcher;

		public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.Accounts;

		public FlowInitializer(IUserSessionProvider userSessionProvider, IDomainQueryResolver domainQueryResolver, IDomainCommandDispatcher domainCommandDispatcher)
        {
            _userSessionProvider = userSessionProvider;
            _domainQueryResolver = domainQueryResolver;
			_domainCommandDispatcher = domainCommandDispatcher;
		}

        public override bool Authorize()
        {
            return !_userSessionProvider.IsAnonymous();
        }

        public override IScreenFlowConfigurator
            OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
                UiFlowContextData contextData)
        {
            var startViewModel = contextData.GetStepData<RootScreenModel>(ScreenName.PreStart);
          

            return preStartCfg
	            .OnEventNavigatesTo(ScreenEvent.ErrorOccurred, CustomerAccountsStep.ShowFlowGenericError)
	            .OnEventNavigatesTo(StepEvent.NoAccountDetected, CustomerAccountsStep.ShowCollectiveAccountError)
	            .OnEventNavigatesTo(StepEvent.DetectedUserHasMultipleAccounts, CustomerAccountsStep.AccountSelection)
	            .OnEventsNavigatesTo(new []{AccountSelection.StepEvent.ToMarketingPreference,AccountSelection.StepEvent.SubmitMeterReading}, CustomerAccountsStep.LoadAccountDashboard)
	            .OnEventNavigatesToAsync(ScreenEvent.Start, CustomerAccountsStep.LoadAccountDashboard,
		            async() => (await _domainQueryResolver.GetAccountInfoByAccountNumber(startViewModel.SelectedUserAccountNumber)).ClientAccountType == ClientAccountType.EnergyService, "Energy Service Account")
	            .OnEventNavigatesToAsync(ScreenEvent.Start, CustomerAccountsStep.LoadAccountDashboard,
		            async() =>
		            {
			            var account =
				            await _domainQueryResolver.GetAccountInfoByAccountNumber(startViewModel.SelectedUserAccountNumber);
			            return account.IsPAYGCustomer && account.ClientAccountType.IsOneOf(ClientAccountType.Electricity,ClientAccountType.Gas);
		            }, "PAYG Customer");
        }


        protected override async Task<RootScreenModel> OnBuildStartData(UiFlowContextData newContext,
	        RootScreenModel data)
        {
	        var getAccounts = _domainQueryResolver.GetAccounts();
	        if (data.AccountNumber != null)
	        {
		        var account = (await getAccounts).SingleOrDefault(x=>x.AccountNumber==data.AccountNumber);
		        data.UserAccountNumbers = account != null ? account.AccountNumber.ToOneItemArray() : new string[0];
	        }

	        
	        var accounts = (await getAccounts).ToArray();
	        if (!data.UserAccountNumbers.Any())
	        {
		        
		        data.UserAccountNumbers = accounts.Select(x=>x.AccountNumber).ToArray();
	        }

	        if (data.UserAccountNumbers.Any())
	        {

				List<Task> enableEbillerTasks = new List<Task>();
				foreach (var businessAgreement in accounts.Where(x => !x.BusinessAgreement.IsEBiller).Select(user => user.BusinessAgreement))
				{
					enableEbillerTasks.Add(_domainCommandDispatcher.ExecuteAsync(new ActivateBusinessAgreementAsEBillerCommand(businessAgreement.BusinessAgreementId)));
				}
				await Task.WhenAll(enableEbillerTasks);
	        }

	        data.UiFlowInitiator = (AppLoginType) (data.Source ?? AppLoginType.Default);
	        data.IsAgentUser =
		        _userSessionProvider.IsAgentOrAdmin();

			return data;
        }

		

        protected override async Task<ScreenEvent> OnResolveInitializationEventToTrigger(ScreenEvent defaultEventToTriggerAfter,
            UiFlowScreenModel screenModel)
        {
	        var result= ScreenEvent.Start;
            var data = (RootScreenModel)screenModel;
            if (data.SelectedUserAccountNumber == null)
            {
	            if (data.UserAccountNumbers.Any())
	            {
		            if (data.UiFlowInitiator == AppLoginType.MarketingPreferences)
		            {
			            result = AccountSelection.StepEvent.ToMarketingPreference;
			            data.SelectedUserAccountNumber = data.UserAccountNumbers.FirstOrDefault();
		            }
		            else if (data.UiFlowInitiator == AppLoginType.MeterReading)
		            {
			            result = AccountSelection.StepEvent.SubmitMeterReading;
			            data.SelectedUserAccountNumber = data.UserAccountNumbers.FirstOrDefault();
		            }
		            else
		            {
			            result = StepEvent.DetectedUserHasMultipleAccounts;
		            }
		           
		            Logger.Debug(() => "User has one or more than one account");
				}
                else
                {
	                result = StepEvent.NoAccountDetected;
	                Logger.Error("User does not have any account");
				}
            }
            

            return result;
        }
		
        public class RootScreenModel : InitialFlowScreenModel, IAccountsFlowInput
		{
            public IEnumerable<string> UserAccountNumbers  { get; set; } = new string[0];
            public string SelectedUserAccountNumber { get; set; }

            /// <summary>
            ///     it defines where the uiflow was originated
            /// </summary>
            public AppLoginType UiFlowInitiator { get; set; }

            public string AccountTypeValue { get; set; }
            public bool IsOpen { get; set; }
            
            public int PageIndex { get; set; }

			//TODO: REDUNDANT??
            public string AccountNumber { get; set; }
            public string Source { get; set; }
            public bool IsAgentUser { get; set; }


			
		}
    }
}