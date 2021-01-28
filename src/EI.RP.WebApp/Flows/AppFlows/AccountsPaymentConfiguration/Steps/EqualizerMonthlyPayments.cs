using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Billing.Activity;
using EI.RP.DomainServices.Queries.Billing.EqualiserPaymentSetupInfo;
using Ei.Rp.Mvc.Core.System;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps
{
	public class EqualizerMonthlyPayments : AccountsPaymentConfigurationScreen
	{
		private const string Title = "Equal Monthly Payments";
		
		private readonly IDomainQueryResolver _domainQueryResolver;

		public static class StepEvent
		{
			public static readonly ScreenEvent SetupEqualizerMonthlyPayments =
				new ScreenEvent(nameof(EqualizerMonthlyPayments),nameof(SetupEqualizerMonthlyPayments));
		}

		public EqualizerMonthlyPayments(IDomainQueryResolver domainQueryResolver)
		{
			_domainQueryResolver = domainQueryResolver;
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.SetupEqualizerMonthlyPayments,
					AccountsPaymentConfigurationStep.SetUpEqualizerMonthlyPayments);
		}


		public override ScreenName ScreenStep => AccountsPaymentConfigurationStep.EqualizerMonthlyPayments;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootData =
				contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart);
			var equaliserSetUpInfo =
				await _domainQueryResolver.GetEqualiserSetUpInfo(rootData.CurrentAccount().Account.AccountNumber);

			var stepData = new ScreenModel();

			SetTitle(Title, stepData);

			if (equaliserSetUpInfo.CanSetUpEqualizer)
			{
				var minDate = DateTime.Today.AddMonths(-12);
				var maxDate = DateTime.Today;
				var invoices =
					(await _domainQueryResolver.GetInvoicesByAccountNumber(equaliserSetUpInfo.AccountNumber, minDate,
						maxDate)).OrderBy(x => x.Amount.Amount).ToArray();
				var lowestInvoice = invoices.FirstOrDefault();
				var highestInvoice = invoices.LastOrDefault();

				stepData.EqualizerMonthlyPaymentDetails = new EqualizerMonthlyPaymentData()
				{
					EqualizerMonthlyPayment = equaliserSetUpInfo.Amount,
					HighestBillInvoiceDate = highestInvoice?.OriginalDate.ToDisplayDate(false),
					HighestBillAmount = highestInvoice?.Amount,
					LowestBillInvoiceDate = lowestInvoice?.OriginalDate.ToDisplayDate(false),
					LowestBillAmount = lowestInvoice?.Amount,
					StartDate = equaliserSetUpInfo.StartDate,
					ContractId = equaliserSetUpInfo.ContractId,
				};
			}

			return stepData;
		}

		public class ScreenModel : UiFlowScreenModel
		{
			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == AccountsPaymentConfigurationStep.EqualizerMonthlyPayments;
			}

			public EqualizerMonthlyPaymentData EqualizerMonthlyPaymentDetails { get; set; }
		}

		public class EqualizerMonthlyPaymentData
		{
			public string HighestBillInvoiceDate { get; set; }

			public string LowestBillInvoiceDate { get; set; }

			public EuroMoney HighestBillAmount { get; set; }

			public EuroMoney LowestBillAmount { get; set; }
			public ClientAccountType AccountType { get; set; }
			public EuroMoney EqualizerMonthlyPayment { get; set; }
			public DateTime? StartDate { get; set; }
			public string ContractId { get; set; }
		}
	}
}