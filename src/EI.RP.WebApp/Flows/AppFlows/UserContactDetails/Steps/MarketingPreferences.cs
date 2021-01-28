using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Commands.Users.MarketingPreferences;
using EI.RP.DomainServices.Queries.User.UserContact;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.UserContactDetails.FlowDefinitions;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.Flows.AppFlows.UserContactDetails.Steps
{
	public class MarketingPreferences : UserContactDetailsScreen
    {
	    protected override string Title => "Marketing";

	    private readonly IDomainQueryResolver _domainQueryResolver;
	    private readonly IDomainCommandDispatcher _domainCommandDispatcher;

		public override ScreenName ScreenStep => UserContactDetailsStep.MarketingPreferences;
		
	    public MarketingPreferences(IDomainQueryResolver domainQueryResolver, 
		    IDomainCommandDispatcher domainCommandDispatcher)
	    {
		    _domainQueryResolver = domainQueryResolver;
		    _domainCommandDispatcher = domainCommandDispatcher;
	    }

	    public static class StepEvent
	    {
		    public static readonly ScreenEvent SaveMarketingPreferences = new ScreenEvent(nameof(UserContactDetailsInput), nameof(SaveMarketingPreferences));
	    }

	    protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
		    IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
	    {
		    return screenConfiguration
			    .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
			    .OnEventReentriesCurrent(StepEvent.SaveMarketingPreferences);
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<UserContactFlowInitializer.RootScreenModel>(ScreenName.PreStart);

			var stepData = new ScreenModel
			{
				AccountNumber = rootData.AccountNumber
			};

			SetTitle(Title, stepData);

			await BuildStepData(stepData);

			return stepData;
		}

		private async Task BuildStepData(ScreenModel screenModel)
		{
			var userContactInfo = await _domainQueryResolver.GetUserContactInfoByAccountNumber(screenModel.AccountNumber);
			
			ResolveCommunicationPreferences();

			void ResolveCommunicationPreferences()
			{
				var communicationPreferences = userContactInfo.CommunicationPreference;

				screenModel.SmsMarketingActive = IsMarketingPreferenceActive(CommunicationPreferenceType.SMS);
				screenModel.LandLineMarketingActive = IsMarketingPreferenceActive(CommunicationPreferenceType.LandLine);
				screenModel.MobileMarketingActive = IsMarketingPreferenceActive(CommunicationPreferenceType.Mobile);
				screenModel.PostMarketingActive = IsMarketingPreferenceActive(CommunicationPreferenceType.Post);
				screenModel.DoorToDoorMarketingActive = IsMarketingPreferenceActive(CommunicationPreferenceType.DoorToDoor);
				screenModel.EmailMarketingActive = IsMarketingPreferenceActive(CommunicationPreferenceType.Email);

				bool IsMarketingPreferenceActive(CommunicationPreferenceType preferenceType)
				{
					var preference = communicationPreferences.SingleOrDefault(x => x.PreferenceType == preferenceType);
					return preference != null && preference.Accepted;
				}
			}
		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.SaveMarketingPreferences)
			{
				var input = contextData.GetCurrentStepData<ScreenModel>();

				await _domainCommandDispatcher.ExecuteAsync(new UpdateMarketingPreferencesCommand(input.AccountNumber,
					input.SmsMarketingActive,
					input.LandLineMarketingActive,
					input.MobileMarketingActive, 
					input.PostMarketingActive,
					input.DoorToDoorMarketingActive,
					input.EmailMarketingActive));

				input.MustShowSuccessfullUpdateMessage = true;
				await BuildStepData(input);
			}
		}
		public class ScreenModel : UiFlowScreenModel
	    {
		    public override bool IsValidFor(ScreenName screenStep)
		    {
			    return screenStep == UserContactDetailsStep.MarketingPreferences;
		    }

		    public string AccountNumber { get; set; }
			
		    public bool SmsMarketingActive { get; set; }

		    public bool DoorToDoorMarketingActive { get; set; }

		    public bool LandLineMarketingActive { get; set; }

		    public bool PostMarketingActive { get; set; }

		    public bool EmailMarketingActive { get; set; }

		    public bool MobileMarketingActive { get; set; }

		    public bool MustShowSuccessfullUpdateMessage { get; set; }
		    public string ErrorMessage { get; set; }
	    }
	}
}
