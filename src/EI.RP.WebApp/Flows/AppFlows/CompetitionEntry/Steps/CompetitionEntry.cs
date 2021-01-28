using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Commands.Users.CompetitionEntry;
using EI.RP.DomainServices.Queries.Competitions;
using EI.RP.DomainServices.Queries.User.UserContact;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.WebApp.Flows.AppFlows.CompetitionEntry.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.Settings;
using EI.RP.WebApp.Infrastructure.Validation.CustomValidators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using Microsoft.AspNetCore.Http;

namespace EI.RP.WebApp.Flows.AppFlows.CompetitionEntry.Steps
{
    public class CompetitionEntry : CompetitionEntryScreen
    {
	    private readonly IDomainQueryResolver _queryResolver;
        private readonly IUiAppSettings _uiAppSettings;
        private readonly IUserSessionProvider _sessionProvider;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IClientInfoResolver _clientInfoResolver;
        private readonly IDomainCommandDispatcher _domainCommandDispatcher;

        public override ScreenName ScreenStep => CompetitionEntryStep.CompetitionEntry;

        public CompetitionEntry(IDomainQueryResolver queryResolver,
            IDomainCommandDispatcher domainCommandDispatcher,
            IUiAppSettings uiAppSettings,
            IUserSessionProvider sessionProvider,
            IHttpContextAccessor contextAccessor,
            IClientInfoResolver clientInfoResolver)
        {
            _queryResolver = queryResolver;
            _uiAppSettings = uiAppSettings;
            _sessionProvider = sessionProvider;
            _contextAccessor = contextAccessor;
            _clientInfoResolver = clientInfoResolver;
            _domainCommandDispatcher = domainCommandDispatcher;
        }

        public static class StepEvent
        {
            public static readonly ScreenEvent SubmitCompetitionEntry = new ScreenEvent(nameof(CompetitionEntry), nameof(SubmitCompetitionEntry));
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventNavigatesTo(StepEvent.SubmitCompetitionEntry, CompetitionEntryStep.CompetitionEntrySuccessful);
        }

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<CompetitionEntryFlowInitializer.RootScreenModel>(ScreenName.PreStart);
            
            var data = new ScreenModel
            {
	            AccountNumber = rootData.AccountNumber,
                Name = _uiAppSettings.CompetitionName,
                Heading = _uiAppSettings.CompetitionHeading,
                Description = _uiAppSettings.CompetitionDescription,
                Description1 = _uiAppSettings.CompetitionDescription1,
                Description2 = _uiAppSettings.CompetitionDescription2,
                Description3 = _uiAppSettings.CompetitionDescription3,
                QuestionPart1 = _uiAppSettings.CompetitionQuestionPart1,
                QuestionPart2 = _uiAppSettings.CompetitionQuestionPart2,
                QuestionPart3 = _uiAppSettings.CompetitionQuestionPart3,
                AnswerA = _uiAppSettings.CompetitionAnswerA,
                AnswerB = _uiAppSettings.CompetitionAnswerB,
                AnswerC = _uiAppSettings.CompetitionAnswerC,
                TermAndConditionsUrl = _uiAppSettings.CompetitionTermAndConditionsUrl,
                AlreadyEnteredMessage = _uiAppSettings.CompetitionAlreadyEnteredMessage,
                ImageDesktop = _uiAppSettings.CompetitionPageImages?.RegularImagePath,
                ImageMobile = _uiAppSettings.CompetitionPageImages?.MobileImagePath,
				ImageAltText = _uiAppSettings.CompetitionPageImages?.AltText
			};

            SetTitle(Title, data);

            return data;
        }

        private async Task BuildStepData(ScreenModel model)
        {
            var competitionEntries = await _queryResolver.GetCompetitionEntriesByUserName(_sessionProvider.CurrentUserClaimsPrincipal.Identity.Name);
            model.HasExistingEntry = competitionEntries?.SingleOrDefault() != null;
        }

        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
            UiFlowScreenModel originalScreenModel,
            IDictionary<string, object> stepViewCustomizations = null)
        {
            var refreshedStepData = originalScreenModel.CloneDeep();
            var updatedStepData = (ScreenModel)refreshedStepData;
            await BuildStepData(updatedStepData);

            SetTitle(Title, updatedStepData);

            return updatedStepData;
        }

        protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
        {
            var screenModel = contextData.GetCurrentStepData<ScreenModel>();
            if (screenModel.HasExistingEntry == true)
            {
                return;
            }

			var accountInfo = await _queryResolver.GetAccountInfoByAccountNumber(screenModel.AccountNumber);
            var userContactInfo = await _queryResolver.GetUserContactInfoByAccountNumber(screenModel.AccountNumber);

            if (triggeredEvent == StepEvent.SubmitCompetitionEntry)
            {
	            try
	            {
		            await _domainCommandDispatcher.ExecuteAsync(new CompetitionEntryCommand(_sessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
	                     screenModel.AccountNumber,
	                     accountInfo.FirstName,
	                     accountInfo.LastName,
	                     userContactInfo.ContactEMail,
	                     userContactInfo.PrimaryPhoneNumber,
	                     screenModel.Name,
	                     DateTime.Now,
	                     screenModel.Answer,
	                     accountInfo.Partner,
	                     false,
	                     _clientInfoResolver.ResolveIp()));
	            }
	            catch (Exception e)
	            {
		            throw new ValidationException("An error has occurred, please try again later.");
	            }
            }
        }

        public class ScreenModel : UiFlowScreenModel
        {
            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == CompetitionEntryStep.CompetitionEntry ||
                       screenStep == CompetitionEntryStep.CompetitionEntrySuccessful;
            }

            public bool? HasExistingEntry { get; set; }
            public string Name { get; set; }
            public string Heading { get; set; }
            public string Description { get; set; }
            public string Description1 { get; set; }
            public string Description2 { get; set; }
            public string Description3 { get; set; }
            public string QuestionPart1 { get; set; }
            public string QuestionPart2 { get; set; }
            public string QuestionPart3 { get; set; }
            public string AnswerA { get; set; }
            public string AnswerB { get; set; }
            public string AnswerC { get; set; }
            public string TermAndConditionsUrl { get; set; }
            public string AlreadyEnteredMessage { get; set; }
            public string ImageDesktop { get; set; }
            public string ImageMobile { get; set; }
            public string ImageAltText { get; set; }

			[Required(AllowEmptyStrings = false, ErrorMessage = "You need to select your answer before you can enter")]
            public string Answer { get; set; }
			[BooleanRequired(ErrorMessage = "You must opt-in to email marketing communications to enter the Electric Ireland customer draw.")]
            public bool Consent { get; set; }

            public string AccountNumber { get; set; }
        }
    }
}
