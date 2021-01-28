using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Commands.Users.UserContact;
using EI.RP.DomainServices.Queries.User.UserContact;
using EI.RP.UiFlows.Core.Flows;

using EI.RP.WebApp.Infrastructure.StringResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.User.PhoneMetaData;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.ErrorHandling;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.DomainModelExtensions;
using EI.RP.WebApp.Flows.AppFlows.UserContactDetails.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.UserContactDetails.Steps
{
	public class UserContactDetailsInput : UserContactDetailsScreen
	{
		private readonly IDomainQueryResolver _domainQueryResolver;
		private readonly IDomainCommandDispatcher _domainCommandDispatcher;

		public UserContactDetailsInput(IDomainQueryResolver domainQueryResolver, IDomainCommandDispatcher domainCommandDispatcher)
		{
			_domainQueryResolver = domainQueryResolver;
			_domainCommandDispatcher = domainCommandDispatcher;
		}

		public override ScreenName ScreenStep => UserContactDetailsStep.UserContactDetails;

		public static class StepEvent
		{
			public static readonly ScreenEvent SaveContactDetails = new ScreenEvent(nameof(UserContactDetailsInput), nameof(SaveContactDetails));
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventReentriesCurrent(StepEvent.SaveContactDetails);
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
			var accountInfoTask = _domainQueryResolver.GetAccountInfoByAccountNumber(screenModel.AccountNumber);

			screenModel.LoginEMail = userContactInfo.LoginEMail;
			screenModel.PrimaryPhoneNumber = userContactInfo.PrimaryPhoneNumber;
			screenModel.AlternativePhoneNumber = userContactInfo.AlternativePhoneNumber;
			screenModel.ContactEMail = userContactInfo.ContactEMail;
			screenModel.PreviousContactEMail = userContactInfo.ContactEMail;
			screenModel.AddressLines = (await accountInfoTask).BusinessAgreement.BillToAccountAddress.AsDescriptionText().Split(',').ToArray();
		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.SaveContactDetails)
			{
				var input = contextData.GetCurrentStepData<ScreenModel>();
				input.MustShowSuccessfullUpdateMessage = false;

				try
				{
					await _domainCommandDispatcher.ExecuteAsync(new UpdateUserContactDetailsCommand(input.AccountNumber,
					input.PrimaryPhoneNumber,
					input.AlternativePhoneNumber,
					input.ContactEMail,
					input.PreviousContactEMail));

					input.MustShowSuccessfullUpdateMessage = true;
				}
                catch (AggregateException ex)
                {
					if (ex.InnerExceptions != null 
						&& ex.InnerExceptions.Any(x => (x as DomainException).DomainError.IsOneOf(ResidentialDomainError.ContactNumberInvalid)))
					{
						throw new ValidationException("Sorry, the contact number provided is invalid.");
					}

					throw;
                }

				await BuildStepData(input);
			}
		}

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var refreshedStepData = originalScreenModel.CloneDeep();
			var updatedStepData = (ScreenModel)refreshedStepData;

			SetTitle(Title, updatedStepData);

			return updatedStepData;
		}

		public class ScreenModel : UiFlowScreenModel
		{
			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == UserContactDetailsStep.UserContactDetails;
			}

			public string AccountNumber { get; set; }

			[Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid email address")]
			[RegularExpression(ReusableRegexPattern.ValidEmail, ErrorMessage = "Please enter a valid email address")]
			public string ContactEMail { get; set; }

			public string PreviousContactEMail { get; set; }

			[Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid phone number")]
			[RegularExpression(ReusableRegexPattern.ValidPhoneNumber, ErrorMessage = "Please enter a valid phone number")]
			public string PrimaryPhoneNumber { get; set; }

			[DisplayFormat(ConvertEmptyStringToNull = false)]
			[RegularExpression(ReusableRegexPattern.ValidPhoneNumber, ErrorMessage = "Please enter a valid phone number")]
			public string AlternativePhoneNumber { get; set; }

			public string LoginEMail { get; set; }
			public IEnumerable<string> AddressLines { get; set; }
			public bool MustShowSuccessfullUpdateMessage { get; set; }
			public string ErrorMessage { get; set; }
		}
	}
}
