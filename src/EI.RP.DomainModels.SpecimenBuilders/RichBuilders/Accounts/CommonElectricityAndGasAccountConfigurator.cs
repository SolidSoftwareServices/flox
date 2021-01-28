using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Dsl;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Banking;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Others;
using Ei.Rp.DomainModels.User;
using EI.RP.DomainServices.Queries.Billing.EqualiserPaymentSetupInfo;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.DomainServices.Queries.Billing.LatestBill;
using EI.RP.DomainServices.Queries.Billing.NextBill;
using EI.RP.DomainServices.Queries.User.UserContact;
using Ei.Rp.DomainModels.MoveHouse;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.MovingHouse.CheckMoveOutRequestResults;
using EI.RP.DomainServices.Queries.MovingHouse.InstalmentPlans;
using EI.RP.DomainServices.Queries.MovingHouse.CheckMprnAddressDetailsInSwitch;
using EI.RP.DomainServices.Queries.SmartActivation.SmartActivationPlan;

namespace EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts
{
	public abstract class CommonElectricityAndGasAccountConfigurator : AccountConfigurator<CommonElectricityAndGasAccountConfigurator>
	{
		private bool _billIsOnPaper;

		private bool _canEstimateNextBill;
		private bool _isContractPending;
		private bool _canSetUpEqualizer;
		private bool _hasAccountCredit;
		private bool _isMoveOutOk;
		private bool _hasExitFee;
		private bool _hasInstalmentPlan;
		private bool _isNewMPRNAddressInSwitch;

		protected CommonElectricityAndGasAccountConfigurator(DomainFacade domainFacade, ClientAccountType accountType) :
			base(domainFacade, accountType)
		{
			FinancialActivitiesConfiguration = new FinancialActivitiesConfiguration(domainFacade);
		}

		public FinancialActivitiesConfiguration FinancialActivitiesConfiguration { get; }

		private bool _asOpened = true;
		private bool _canBeRefunded;
		public AddNewDuelFuelAccountConfigurator NewDuelFuelAccountConfigurator { get; private set; } = null;

		private DateTime[] _withSetUpEqualizerTestQueryDates;
		private CommonElectricityAndGasAccountConfigurator _duelFuelSisterAccount;
		private DateTime? _contractStartDate;
		private DateTime? _contractEndDate;
		private bool _isLossInProgress;
		private bool _hasStaffDiscount;
		private bool _hasFreeElectricityAllowance;
		private string _planName;
		private decimal _discount;
		
		private bool _hasQuotationsInProgress;
		private bool _canMoveToStandardPlan;
		private bool _switchToSmartPlanDismissed;
		

		public CommonElectricityAndGasAccountConfigurator WithOpenedStatus(bool opened)
		{
			_asOpened = opened;
			return this;
		}
		public CommonElectricityAndGasAccountConfigurator WithPlan(string planName, decimal discount)
		{
			_planName = planName;
			_discount = discount;
			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithEqualizerSetUpAvailable(bool setUpEqualizer)
		{
			_canSetUpEqualizer = setUpEqualizer;
			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithBillsOnPaper(bool billIsOnPaper)
		{
			_billIsOnPaper = billIsOnPaper;
			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithIsContractPending(bool isContractPending)
		{
			_isContractPending = isContractPending;
			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithLatestBill(bool canEstimateLatestBill)
		{
			_canEstimateNextBill = canEstimateLatestBill;
			return this;
		}
		public CommonElectricityAndGasAccountConfigurator WithContractStartDate(DateTime? contractStartDate)
		{
			_contractStartDate = contractStartDate;

			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithContractEndDate(DateTime? contractEndDate)
		{
			_contractEndDate = contractEndDate;

			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithIsLossInProgress(bool isLossInProgress)
		{
			_isLossInProgress = isLossInProgress;

			return this;
		}


		protected override IPostprocessComposer<AccountInfo> ConfigureSpecificTypeAccountInfo(
			IPostprocessComposer<AccountInfo> accountInfoComposer)
		{
			var composer = accountInfoComposer.With(x => x.ContractId, _contractId)
				.With(x => x.IsOpen, _asOpened)
				.With(x => x.PaperBillChoice, _billIsOnPaper ? PaperBillChoice.On : PaperBillChoice.Off)
				.With(x => x.PaymentMethod, PaymentType)
				.With(x => x.IsLossInProgress, _isLossInProgress)
				.With(x => x.DiscountAppliedPercentage, _discount)
				.With(x => x.HasQuotationsInProgress, _hasQuotationsInProgress)
				.With(x=>x.CanSwitchToStandardPlan,_canMoveToStandardPlan)
				.With(x=>x.HasStaffDiscountApplied,_hasStaffDiscount)
				.With(x => x.ContractStatus, _isContractPending ? ContractStatusType.Pending : ContractStatusType.Active)
				.With(x=>x.SwitchToSmartPlanDismissed,_switchToSmartPlanDismissed);

				
				
			if (_contractStartDate != null)
			{
				composer = composer.With(x => x.ContractStartDate, _contractStartDate);
			}

			if (_contractEndDate != null)
			{
				composer = composer.With(x => x.ContractEndDate, _contractEndDate);
			}

			if (_planName != null)
			{
				composer = composer.With(x => x.PlanName, _planName);
			}
			
			return composer;

			
		}

		protected override void ConfigureDependenciesForSpecificAccountType(AccountInfo accountInfoModel)
		{

			ConfigureLatestBill();
			ConfigureEstimationBill();
			ConfigureGeneralBillInfo();
			ConfigureEqualizerDetails();

			ConfigureUserAccount();

			ConfigureDuelFuelAccounts();

			ConfigurePremises();
			ConfigureMoveOutQuery();
			ConfigureInstalmentPlanQuery();
			ConfigureCheckMprnAddressDetailsInSwitchQueryQuery();
			FinancialActivitiesConfiguration.Execute(_accountNumber, PaymentType);
		}

		public CommonElectricityAndGasAccountConfigurator WithSmartPlans()
		{
			SmartPlans = DomainFacade.ModelsBuilder.Build<SmartPlan>()
				.With(x => x.IsActive, true)
				.With(x => x.FreeDayOfElectricityChoice, new DayOfWeek[0])
				.Without(x => x.FreeDayOfElectricityDescription)
				.CreateMany(3).Select((x, idx) =>
				{
					x.OrderIndex = idx + 1;
					return x;
				}).ToArray();
			var smartPlan = SmartPlans[1];
			smartPlan.FreeDayOfElectricityChoice = new[] {DayOfWeek.Saturday, DayOfWeek.Sunday};
			smartPlan.FreeDayOfElectricityDescription = DomainFacade.ModelsBuilder.Create<string>();
			DomainFacade.QueryResolver.ExpectQuery(new SmartActivationPlanQuery
				{
					AccountNumber = _accountNumber,OnlyActive = true
				},SmartPlans
				);
			return this;
		}

		public SmartPlan[] SmartPlans { get; set; }

		private void ConfigurePremises()
		{
			Premise.WithStaffDiscount(_hasStaffDiscount);
			Premise.WithFreeElectricityAllowance(_hasFreeElectricityAllowance);
			Premise.Execute();
			NewPremise.Execute();
			Premise.SetAsCurrentForAccount(_accountNumber);
			if (_duelFuelSisterAccount != null)
			{
				Premise = _duelFuelSisterAccount.Premise.SetDualPrn(Premise);
				NewPremise = _duelFuelSisterAccount.NewPremise.SetDualPrn(NewPremise);
			}

			if (Model.IsOpen && Model.IsElectricityAccount() && Premise?.Devices?.Any() == true)
			{
				Model.SmartActivationStatus = Premise.Devices.Max(x => x.SmartActivationStatus);
			}

			if (Model.IsOpen && Model.IsElectricityAccount() && Premise?.Devices?.Any() == true)
			{
				Model.CanSubmitMeterReading = !Premise.Devices.Any(x =>
					x.MCCConfiguration == RegisterConfigType.MCC12 ||
					(x.CTF != null &&
					 x.CTF.IsOneOf(CommsTechnicallyFeasibleValue.CTF2,
						 CommsTechnicallyFeasibleValue.CTF3,
						 CommsTechnicallyFeasibleValue.CTF4) &&
					 x.MCCConfiguration != null &&
					 x.MCCConfiguration.IsOneOf(RegisterConfigType.MCC01,
						 RegisterConfigType.MCC16)));
			}
			else if (Model.IsOpen && Model.IsGasAccount())
			{
				Model.CanSubmitMeterReading = true;
			}

		}



		private void ConfigureLatestBill()
		{
			LatestBill = DomainFacade.ModelsBuilder.Build<LatestBillInfo>()
				.With(x => x.AccountNumber, _accountNumber)
				.With(x => x.AccountIsOpen, true)

				//TODO: CONFIGURATION VALUES???
				.With(x => x.MeterReadingType, MeterReadingCategoryType.Actual)
				.With(x => x.PaymentMethod, PaymentMethodType.DirectDebit)
				.With(x => x.CostCanBeEstimated, _canEstimateNextBill)
				.With(x => x.HasAccountCredit, _hasAccountCredit)
				.With(x => x.CanRequestRefund, _canBeRefunded)
				.With(x => x.CanAddGasAccount, NewDuelFuelAccountConfigurator != null)

				.Create();
			DomainFacade
				.QueryResolver
				.ExpectQuery(new LatestBillQuery
				{
					AccountNumber = _accountNumber,
				}, LatestBill.ToOneItemArray());
		}

		private void ConfigureGeneralBillInfo()
		{
			BillingInfo = DomainFacade.ModelsBuilder.Build<GeneralBillingInfo>()
				//TODO: CONFIGURATION VALUES???
				.With(x => x.MeterReadingType, MeterReadingCategoryType.Actual)
				.With(x => x.PaymentMethod, PaymentMethodType.DirectDebit)
				.Create();
			DomainFacade
				.QueryResolver
				.ExpectQuery(new GeneralBillingInfoQuery
				{
					AccountNumber = _accountNumber,
				}, BillingInfo.ToOneItemArray());
		}


		private void ConfigureEstimationBill()
		{

			NextBillEstimation = DomainFacade.ModelsBuilder.Build<NextBillEstimation>()
				.With(x => x.AccountNumber, _accountNumber)
				.With(x => x.CostCanBeEstimated, _canEstimateNextBill)
				.Create();

			DomainFacade
				.QueryResolver
				.ExpectQuery(new EstimateNextBillQuery
				{
					AccountNumber = _accountNumber,
				}, NextBillEstimation.ToOneItemArray());
		}



		private void ConfigureUserAccount()
		{
			UserContactDetails = DomainFacade.ModelsBuilder.Build<UserContactDetails>()
				.With(x => x.AccountNumber, _accountNumber)
				.With(x => x.ContactEMail, DomainFacade.ModelsBuilder.Create<EmailAddress>().ToString)
				.With(x => x.PrimaryPhoneNumber, "0892332458")
				.With(x => x.AlternativePhoneNumber, "0892332458")
				.With(x => x.CommunicationPreference, CommunicationPreferenceType.AllValues.Select(x =>
					new CommunicationPreference
					{
						Accepted = DomainFacade.ModelsBuilder.Create<bool>(),
						PreferenceType = (CommunicationPreferenceType)x
					}))
				.Create();

			DomainFacade.QueryResolver.ExpectQuery(new UserContactDetailsQuery
			{
				AccountNumber = _accountNumber
			}, UserContactDetails.ToOneItemArray());
		}

		private void ConfigureEqualizerDetails()
		{
			DomainFacade.QueryResolver.ExpectQuery(new EqualizerPaymentSetupInfoQuery
			{
				AccountNumber = _accountNumber,
			}, DomainFacade.ModelsBuilder.Build<EqualizerPaymentSetupInfo>()
				.With(x => x.CanSetUpEqualizer, _canSetUpEqualizer)
				.With(x => x.AccountNumber, _accountNumber)
				.With(x => x.Amount, DomainFacade.ModelsBuilder.Create<EuroMoney>())
				.Create().ToOneItemArray());


			if (_canSetUpEqualizer)
			{
				foreach (var queryableDate in _withSetUpEqualizerTestQueryDates)
				{
					EqualizerPaymentSetupData.Add(queryableDate, DomainFacade.ModelsBuilder.Build<EqualizerPaymentSetupInfo>()
						.With(x => x.CanSetUpEqualizer, _canSetUpEqualizer)
						.With(x => x.AccountNumber, _accountNumber)
						.With(x => x.Amount, DomainFacade.ModelsBuilder.Create<EuroMoney>())
						.Create());

					DomainFacade.QueryResolver.ExpectQuery(new EqualizerPaymentSetupInfoQuery
					{
						AccountNumber = _accountNumber,
						FirstPaymentDateTime = queryableDate
					}, EqualizerPaymentSetupData[queryableDate].ToOneItemArray());
				}
			}
		}


		private void ConfigureDuelFuelAccounts()
		{
			var accountInfos = Model.ToOneItemArray();

			if (_duelFuelSisterAccount != null)
			{
				accountInfos = new[] { Model, _duelFuelSisterAccount.Model };
				//sets the other
				DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
				{
					AccountNumber = _duelFuelSisterAccount._accountNumber,
					RetrieveDuelFuelSisterAccounts = true
				}, accountInfos);
				DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
				{
					AccountNumber = _duelFuelSisterAccount._accountNumber,
					RetrieveDuelFuelSisterAccounts = true
				}, accountInfos.Select(x=>(AccountOverview)x).ToArray());
			}

			DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
			{
				AccountNumber = _accountNumber,
				RetrieveDuelFuelSisterAccounts = true
			}, accountInfos);
			DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
			{
				AccountNumber = _accountNumber,
				RetrieveDuelFuelSisterAccounts = true
			}, accountInfos.Select(x=>(AccountOverview)x).ToArray());

			NewDuelFuelAccountConfigurator?.Execute();
		}

		private void ConfigureMoveOutQuery()
		{
			DomainFacade.QueryResolver.ExpectQuery(new CheckMoveOutRequestResultQuery
			{
				ContractID = _contractId
			},
				new CheckMoveOutRequestResult() { IsMoveOutOk = _isMoveOutOk, HasExitFee = _hasExitFee }.ToOneItemArray());
		}

		private void ConfigureInstalmentPlanQuery()
		{
			DomainFacade.QueryResolver.ExpectQuery(new InstalmentPlanQuery
			{
				AccountNumber = _accountNumber
			},
				new InstalmentPlanRequestResult() { HasInstalmentPlan = _hasInstalmentPlan }.ToOneItemArray());
		}

		private void ConfigureCheckMprnAddressDetailsInSwitchQueryQuery()
		{
			DomainFacade.QueryResolver.ExpectQuery(new CheckMprnAddressDetailsInSwitchQuery
			{
				MPRN = (string)NewPremise.GetPrn(AccountType)
			},
				new CheckMprnAddressDetailsInSwitchResult() { HasAddressDetails = _isNewMPRNAddressInSwitch }.ToOneItemArray());
		}

		public CommonElectricityAndGasAccountConfigurator WithInvoices(int numInvoices, DateTime? minDate = null,
			DateTime? maxDate = null)
		{
			FinancialActivitiesConfiguration.WithInvoices(numInvoices, minDate, maxDate);

			return this;
		}

		public UserContactDetails UserContactDetails { get; set; }
		public LatestBillInfo LatestBill { get; private set; }
		public GeneralBillingInfo BillingInfo { get; private set; }

		public NextBillEstimation NextBillEstimation { get; private set; }

		public Dictionary<DateTime, EqualizerPaymentSetupInfo> EqualizerPaymentSetupData { get; private set; } =
			new Dictionary<DateTime, EqualizerPaymentSetupInfo>();

		public AddressInfo AddressInfo { get; private set; }


		public CommonElectricityAndGasAccountConfigurator WithEBiller(bool isEbiller)
		{
			IsEBiller = isEbiller;
			return this;
		}


		public CommonElectricityAndGasAccountConfigurator WithPaymentMethodType(PaymentMethodType paymentType)
		{
			PaymentType = paymentType;
			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithEqualMonthlyPayments(params DateTime[] withTestQueryDates)
		{
			if (withTestQueryDates.Length == 0)
				throw new ArgumentException("Value cannot be an empty collection.", nameof(withTestQueryDates));
			_canSetUpEqualizer = true;

			_withSetUpEqualizerTestQueryDates = withTestQueryDates.Distinct().ToArray();
			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithAccountCredit(bool hasAccountCredit)
		{
			_hasAccountCredit = hasAccountCredit;
			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithRefundLinkAvailable(bool canBeRefunded)
		{
			_canBeRefunded = canBeRefunded;
			return this;
		}
		/// <summary>
		/// configure to allow adding a new account
		/// </summary>
		/// <returns></returns>
		public CommonElectricityAndGasAccountConfigurator WithAddNewAccountAvailable(bool addressExists, bool newAccountPrnDeregistered)
		{
			var newAccountType = AccountType == ClientAccountType.Electricity
				? ClientAccountType.Gas
				: AccountType == ClientAccountType.Gas
					? ClientAccountType.Electricity
					: throw new NotSupportedException();
			NewPremise.NewDuelFuelAccountConfigurator = NewDuelFuelAccountConfigurator = new AddNewDuelFuelAccountConfigurator(DomainFacade, newAccountType, addressExists, newAccountPrnDeregistered);

			return this;
		}


		public CommonElectricityAndGasAccountConfigurator HavingDuelFuelWith(
			CommonElectricityAndGasAccountConfigurator duelFuelSisterAccount)
		{
			Premise.DuelFuelSisterAccount = _duelFuelSisterAccount = duelFuelSisterAccount;
			return this;
		}


		public CommonElectricityAndGasAccountConfigurator WithDeviceSet(ConfigurableDeviceInfo deviceSet)
		{
			Premise.WithDevicesForConfiguration(deviceSet);
			NewPremise.WithDevicesForConfiguration(deviceSet);
			return this;
		}
		public CommonElectricityAndGasAccountConfigurator WithDeviceSet(ConfigurableDeviceSet deviceSet,
			RegisterConfigType configType = null, CommsTechnicallyFeasibleValue ctfValue = null)
		{
			return this.WithDeviceSet(new ConfigurableDeviceInfo(deviceSet, configType, ctfValue));
		}
		public CommonElectricityAndGasAccountConfigurator WithMoveOutOk(bool isMoveOutOk)
		{
			_isMoveOutOk = isMoveOutOk;
			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithExitFee(bool hasExitFee)
		{
			_hasExitFee = hasExitFee;
			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithInstalmentPlan(bool hasInstalmentPlan)
		{
			_hasInstalmentPlan = hasInstalmentPlan;
			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithNewMPRNAddressInSwitch(bool isNewMPRNAddressInSwitch)
		{
			_isNewMPRNAddressInSwitch = isNewMPRNAddressInSwitch;
			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithStaffDiscount(bool hasStaffDiscount)
		{
			_hasStaffDiscount = hasStaffDiscount;
			return this;
		}

		public CommonElectricityAndGasAccountConfigurator WithFreeElectricityAllowance(bool hasFreeElectricityAllowance)
		{
			_hasFreeElectricityAllowance = hasFreeElectricityAllowance;
			return this;
		}

	
		public virtual CommonElectricityAndGasAccountConfigurator WithQuotationsInProgress(bool hasQuotationsInProgress)
		{
			_hasQuotationsInProgress = hasQuotationsInProgress;
			return this;
		}

		public virtual CommonElectricityAndGasAccountConfigurator WithCanMoveToStandardPlan(bool canMoveToStandardPlan)
		{
			_canMoveToStandardPlan = canMoveToStandardPlan;
			return this;
		}

		public virtual CommonElectricityAndGasAccountConfigurator WithSwitchToSmartPlanDismissed(bool switchToSmartPlanDismissed)
		{
			_switchToSmartPlanDismissed = switchToSmartPlanDismissed;
			return this;
		}
	}
}