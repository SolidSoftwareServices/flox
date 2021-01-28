using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Billing.EqualiserPaymentSetupInfo;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.WebApp.Flows.AppFlows.Plan.Components.AccountBilling
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		private readonly IDomainQueryResolver _domainQueryResolver;

		public ViewModelBuilder(IDomainQueryResolver queryResolver)
		{
			_domainQueryResolver = queryResolver;
		}

		public async Task<ViewModel> Resolve(InputModel componentInput,
			UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			var getAccount = _domainQueryResolver.GetAccountInfoByAccountNumber(componentInput.AccountNumber);
			var getBillingInfo= _domainQueryResolver.GetAccountBillingInfoByAccountNumber(componentInput.AccountNumber);
			var getEqualizerSetUpInfo = _domainQueryResolver.GetEqualiserSetUpInfo(componentInput.AccountNumber);

			var accountInfo = await getAccount;
			var isContractStatusPending = accountInfo.ContractStatus == ContractStatusType.Pending;

			var result = new ViewModel
			{
				AccountNumber = componentInput.AccountNumber,
				PaymentMethod = accountInfo.PaymentMethod,
				IsContractPending = isContractStatusPending
			};
			
			await MapDirectDebitInfo();
			await MapEqualiser();
			MapPaperBillSettings();
			await MapMonthlyBilling();
			await MapMeterData();

			return result;

			async Task MapDirectDebitInfo()
			{
				if (result.PaymentMethod.IsOneOf(PaymentMethodType.DirectDebit, PaymentMethodType.Equalizer))
				{
					var currentAccount = accountInfo.IncomingBankAccount;

					result.DirectDebit = new ViewModel.DirectDebitInfo
					{
						IBAN = currentAccount.IBAN,
						NameInBankAccount = currentAccount.NameInBankAccount
					};
				}
			}

			async Task MapEqualiser()
			{
				var equaliser = result.Equaliser;
				equaliser.IsVisible = (await getEqualizerSetUpInfo).CanSetUpEqualizer;
				equaliser.IsContractPending = isContractStatusPending;
			}

			void MapPaperBillSettings()
			{
				var paperBill = result.PaperBill;
				paperBill.IsVisible = accountInfo.IsOpen && accountInfo.IsElectricityAccount();
				paperBill.SwitchOffPaperBillEvent = componentInput.SwitchOffPaperBillEvent;
				paperBill.SwitchOnPaperBillEvent = componentInput.SwitchOnPaperBillEvent;
				paperBill.HasPaperBill = accountInfo.PaperBillChoice.ToBoolean();
				paperBill.IsContractPending = isContractStatusPending;
				paperBill.IsSmartAccount = accountInfo.IsSmart();
			}

			async Task MapMonthlyBilling()
			{
				var monthlyBilling = result.MonthlyBilling;
				monthlyBilling.SwitchOnMonthlyBilling = componentInput.SwitchOnMonthlyBillingEvent;
				monthlyBilling.SwitchOffMonthlyBilling = componentInput.SwitchOffMonthlyBillingEvent;

				var billingInfo = (await getBillingInfo).MonthlyBilling;
				monthlyBilling.IsMonthlyBillingActive = billingInfo.IsMonthlyBillingActive;
				monthlyBilling.CanSwitchToMonthlyBilling = billingInfo.CanSwitchToMonthlyBilling;
				monthlyBilling.IsVisible = accountInfo.IsSmart() && accountInfo.PaperBillChoice.ToBoolean();
				monthlyBilling.IsContractPending = isContractStatusPending;
				monthlyBilling.CanEnableMonthlyPayments = accountInfo.IsElectricityAccount()
				                                          && accountInfo.IsOpen
				                                          && accountInfo.IsSmart();
				result.MonthlyBillingDayOfTheMonth = billingInfo.MonthlyBillingDayOfMonth;
			}

			async Task MapMeterData()
			{
				var meterData = result.MeterData;
				result.NoAccessToFeatures = componentInput.NoAccessToFeatures;
				result.MovedToStandardPlan = componentInput.MovedToStandardPlan;
				result.AgreeTermsAndConditions = componentInput.AgreeTermsAndConditions;

				var meterDataFields = new [] { nameof(ViewModel.NoAccessToFeatures), nameof(ViewModel.MovedToStandardPlan), nameof(ViewModel.AgreeTermsAndConditions) };
				meterData.HasValidationErrors = screenModelContainingTheComponent!=null ? screenModelContainingTheComponent
					.Errors
					.Any(x => 
						meterDataFields.Any(y => y == x.MemberName)) : false;

				meterData.SwitchOffMeterData = componentInput.SwitchOffMeterData;

			

				meterData.IsVisible = accountInfo.CanSwitchToStandardPlan;
				meterData.IsContractPending = isContractStatusPending;
				if ( accountInfo.CanSwitchToStandardPlan)
				{
					var moveToStandardPlanSubmittedAlready = componentInput.IsMoveToStandardPlanRequestSendSuccesfully;
					meterData.IsActive = !(moveToStandardPlanSubmittedAlready || accountInfo.HasQuotationsInProgress);
				} 

				meterData.IsChecked = true;
				meterData.PlanName = string.IsNullOrEmpty(accountInfo.BundleReference) ? "Home Electric" : "Home Dual";				
			}
		}
	}
}