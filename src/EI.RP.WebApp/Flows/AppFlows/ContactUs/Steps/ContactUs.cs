using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Users.ContactUs;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using Ei.Rp.Mvc.Core.ViewModels.Validations;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.DomainModelExtensions;
using EI.RP.WebApp.Flows.AppFlows.ContactUs.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.Extensions;
using EI.RP.WebApp.Infrastructure.StringResources;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EI.RP.WebApp.Flows.AppFlows.ContactUs.Steps
{
	public class ContactUs : ContactUsScreen
	{
		private readonly IDomainCommandDispatcher _domainCommandDispatcher;
		private readonly IDomainQueryResolver _domainQueryResolver;

		public ContactUs(IDomainQueryResolver domainQueryResolver,
			IDomainCommandDispatcher domainCommandDispatcher)
		{
			_domainQueryResolver = domainQueryResolver;
			_domainCommandDispatcher = domainCommandDispatcher;
		}

		public override ScreenName ScreenStep => ContactUsStep.ContactUs;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.SubmitQuery, ContactUsStep.ShowContactUsStatusMessage)
				.OnEventReentriesCurrent(StepEvent.AccountChanged)
				.OnEventReentriesCurrent(StepEvent.QueryChanged);
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<ContactUsFlowInitializer.RootScreenModel>(ScreenName.PreStart);

			var stepData = new ScreenModel
			{
				QueryTypes = GetTypeOfQueryDetails()
			};

			SetTitle(Title, stepData);

			if (string.IsNullOrWhiteSpace(rootData.AccountNumber)) return stepData;

			var accounts = (await _domainQueryResolver.GetAccounts()).ToArray();
			stepData.HasAccounts = accounts.Any();
			stepData.AccountList =
				accounts.Select(accountInfo =>
					new SelectListItem
					{
						Text = string.Join(" ", accountInfo.ClientAccountType, accountInfo.Description),
						Value = accountInfo.AccountNumber
					}).ToList();

			var selectedAccount = string.IsNullOrWhiteSpace(rootData.AccountNumber)
				? accounts.First()
				: accounts.First(x => x.AccountNumber == rootData.AccountNumber);

			stepData.SelectedAccount = selectedAccount.AccountNumber;
			stepData.Partner = selectedAccount.Partner;

			return stepData;
		}

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var result = false;
			var refreshedStepData = originalScreenModel.CloneDeep();
			var updatedStepData = (ScreenModel) refreshedStepData;

			SetTitle(Title, updatedStepData);

			if (stepViewCustomizations != null)
			{
				result |= updatedStepData.SetFlowCustomizableValue(stepViewCustomizations, x => x.SelectedQueryType);
				result |= updatedStepData.SetFlowCustomizableValue(stepViewCustomizations, x => x.SelectedAccount);
			}

			await BuildComponents(updatedStepData);

			updatedStepData.ErrorMessage = contextData.GetLastError()?.Trim();

			return updatedStepData;
		}

		private async Task BuildComponents(ScreenModel screenModel)
		{
			if (!string.IsNullOrWhiteSpace(screenModel.SelectedAccount))
			{
				var accountInfo = await _domainQueryResolver.GetAccountInfoByAccountNumber(screenModel.SelectedAccount);
				screenModel.Partner = accountInfo.Partner;
			}
		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			var input = contextData.GetCurrentStepData<ScreenModel>();

			if (input.DontValidateEvents.Contains(triggeredEvent)) contextData.LastError = null;

			if (triggeredEvent == StepEvent.SubmitQuery)
			{
				var queryType = (ContactQueryType) Uri.UnescapeDataString(input.SelectedQueryType);

				var accountNumber = queryType == ContactQueryType.AddAdditionalAccount
					? input.AccountNumber
					: input.SelectedAccount;
				try
				{
					await _domainCommandDispatcher.ExecuteAsync(new UserContactRequest(input.Partner, accountNumber,
						input.MPRN,
						input.Subject, input.CommentText, queryType));
				}
				catch (AggregateException ex)
				{
					if (ex.InnerExceptions == null || ex.InnerExceptions.Any(x =>
					{
						var domainException = x as DomainException;
						if (domainException != null &&
						    domainException.DomainError.Equals(ResidentialDomainError.AccAlreadyRegisteredError))
							throw new DomainException(ResidentialDomainError.AccAlreadyRegisteredError,
								"Sorry there has been an error. Please try again in a few moments.");

						return domainException == null ||
						       !domainException.DomainError.Equals(ResidentialDomainError.InvalidBusinessAgreement);
					}))
						throw;
				}
			}
		}

		private static IEnumerable<SelectListItem> GetTypeOfQueryDetails()
		{
			return new SelectListItem("Please select", "")
				.ToOneItemArray()
				.Union(ContactQueryType.AllValues.Cast<ContactQueryType>()
					.Select(x =>
						new SelectListItem
						{
							Text = x.ToDisplayValue(),
							Value = Uri.EscapeDataString(x.ToString())
						})).ToArray();
		}

		public static class StepEvent
		{
			public static readonly ScreenEvent SubmitQuery = new ScreenEvent(nameof(ContactUs), nameof(SubmitQuery));

			public static readonly ScreenEvent AccountChanged =
				new ScreenEvent(nameof(ContactUs), nameof(AccountChanged));

			public static readonly ScreenEvent QueryChanged = new ScreenEvent(nameof(ContactUs), nameof(QueryChanged));
		}

		public class ScreenModel : UiFlowScreenModel
		{
			public override IEnumerable<ScreenEvent> DontValidateEvents =>
				base.DontValidateEvents.Union(new[] {StepEvent.AccountChanged, StepEvent.QueryChanged});


			public IEnumerable<SelectListItem> AccountList { get; set; }

			[Required(AllowEmptyStrings = false, ErrorMessage = "Please choose an account.")]
			public string SelectedAccount { get; set; }

			public IEnumerable<SelectListItem> QueryTypes { get; set; }

			[Required(AllowEmptyStrings = false, ErrorMessage = "Please choose your query type.")]
			public string SelectedQueryType { get; set; }

			[Required(AllowEmptyStrings = false, ErrorMessage = "Please give a short description of your query. ")]
			[RegularExpression(ReusableRegexPattern.ValidSubject,
				ErrorMessage = "Please give a valid description of your query.")]
			public string Subject { get; set; }

			[Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your message here.")]
			[RegularExpression(ReusableRegexPattern.ValidAccountQuery,
				ErrorMessage = "Please enter your message here.")]
			[StringLength(1900, ErrorMessage = "Please enter no more than 1900 characters")]
			public string CommentText { get; set; }

			public bool AccountDetailsRequired
			{
				get
				{
					if (!string.IsNullOrEmpty(SelectedQueryType))
						return (ContactQueryType) Uri.UnescapeDataString(SelectedQueryType) ==
						       ContactQueryType.AddAdditionalAccount;

					return false;
				}
			}

			[RequiredIf(nameof(AccountDetailsRequired), IfValue = true,
				ErrorMessage = "Please enter your Electric Ireland account number")]
			[RegularExpression(ReusableRegexPattern.ValidAccountNumber,
				ErrorMessage = "Please enter your Electric Ireland account number ")]
			[StringLength(12, ErrorMessage = "Invalid Account Length")]
			public string AccountNumber { get; set; }

			[RequiredIf(nameof(AccountDetailsRequired), IfValue = true,
				ErrorMessage = "Please enter a valid MPRN or GPRN")]
			[RegularExpression(ElectricityPointReferenceNumber.OldMPRNRegEx,
				ErrorMessage = "Please enter a valid MPRN or GPRN ")]
			public string MPRN { get; set; }

			public string ErrorMessage { get; set; }

			public string Partner { get; set; }

			public bool HasAccounts { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == ContactUsStep.ContactUs;
			}
		}
	}
}