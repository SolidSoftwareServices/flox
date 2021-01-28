using AutoFixture;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.DomainServices.Queries.Billing.LatestBill;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.WebApp.DomainModelExtensions;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.DirectDebit;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.DomainServices.Queries.Billing.EqualiserPaymentSetupInfo;
using Ei.Rp.DomainModels.Quotations;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Plan
{
	internal class WhenInPlanPageTests : MyAccountCommonTests<PlanPage>
	{
		protected virtual bool IsSmartAccount => false;
		protected virtual PaymentMethodType PaymentType => PaymentMethodType.Manual;

		protected virtual bool CanAddGasAccount => false;
		protected virtual bool IsUpgradeToSmartAvailable => false;
		protected virtual bool CanSmartMeterMoveToToStandardPlan => false;
		protected virtual bool IsSmartMeterOnChangeToStandardPlan => false;
		protected virtual bool IsDirectDebit => true;
		protected virtual bool WithPaperBill => true;
		protected virtual bool WithIsContractPending => false;

		protected virtual bool IsMonthlyBillingActive => false;
		protected virtual bool CanSwitchToMonthlyBilling => false;
		protected virtual int? MonthlyBillingDayOfMonth => null;


		private string NameInBankAccount;
		private string Iban;

		protected void SetupMocks()
		{
			var account = UserConfig.Accounts.First();
			App.DomainFacade.QueryResolver.ExpectQuery(new LatestBillQuery
			{
				AccountNumber = account.AccountNumber
			},
				Fixture
					.Build<LatestBillInfo>()
					.With(x => x.CanAddGasAccount, CanAddGasAccount)
					.Create()
					.ToOneItemArray()
			);

			if (IsDirectDebit)
			{
				var firstAccount = UserConfig.Accounts.FirstOrDefault();

				NameInBankAccount = firstAccount?.IncomingBankAccount?.NameInBankAccount;
				Iban = firstAccount?.IncomingBankAccount?.IBAN;
			}

			if (IsSmartAccount)
			{
				App.DomainFacade.QueryResolver.ExpectQuery(new GeneralBillingInfoQuery
				{
					AccountNumber = account.AccountNumber
				},
					Fixture
						.Build<GeneralBillingInfo>()
						.With(x => x.MonthlyBilling, Fixture
							.Build<GeneralBillingInfo.MonthlyBillingInfo>()
							.With(mb => mb.IsMonthlyBillingActive, IsMonthlyBillingActive)
							.With(mb => mb.CanSwitchToMonthlyBilling, CanSwitchToMonthlyBilling)
							.With(mb => mb.MonthlyBillingDayOfMonth, MonthlyBillingDayOfMonth)
							.Create())
						.Create()
						.ToOneItemArray()
				);
			}

			App.DomainFacade.QueryResolver.ExpectQuery(new EqualizerPaymentSetupInfoQuery
			{
				AccountNumber = UserConfig.Accounts.First().AccountNumber
			},
				Fixture
					.Build<EqualizerPaymentSetupInfo>()
					.With(x => x.CanSetUpEqualizer, !IsSmartAccount)
					.Create()
					.ToOneItemArray()
			);


		}

		private DeviceInfo CreateDevice(bool isSmart, DivisionType deviceDivisionType, RegisterConfigType mccConfig, CommsTechnicallyFeasibleValue ctf)
		{
			return Fixture
				.Build<DeviceInfo>()
				.With(device => device.IsSmart, isSmart)
				.With(device => device.DivisionId, deviceDivisionType)
				.With(device => device.MCCConfiguration, mccConfig) //MCC01 or MCC16 for CanOptToSmartActive
				.With(device => device.CTF, ctf) //CTF3 or CTF4 for AllowsAllSmartFeatures
				.With(device => device.SmartActivationStatus, ResolveSmartActivation())
				.Create();

			SmartActivationStatus ResolveSmartActivation()
			{
				var smartActivationStatus = SmartActivationStatus.SmartNotAvailable;
				if (IsUpgradeToSmartAvailable && deviceDivisionType == DivisionType.Electricity)
				{
					if (mccConfig != null && mccConfig.IsSmartConfigurationActive())
					{
						smartActivationStatus = SmartActivationStatus.SmartActive;
					}
					else if (mccConfig != null && mccConfig.CanOptToSmartActive())
					{
						smartActivationStatus = ctf.AllowsAllSmartFeatures() ? SmartActivationStatus.SmartAndEligible : SmartActivationStatus.SmartButNotEligible;
					}
				}

				return smartActivationStatus;
			}
		}

		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			var electricAccountConfig = UserConfig.AddElectricityAccount(
				configureDefaultDevice: IsSmartAccount || (!IsUpgradeToSmartAvailable && !CanSmartMeterMoveToToStandardPlan),
				withPaperBill: WithPaperBill,
				isContractPending: WithIsContractPending,
				isSmart: IsSmartAccount,
				paymentType: PaymentType,
				hasQuotationsInProgress: IsSmartMeterOnChangeToStandardPlan,
				canMoveToStandardPlan: CanSmartMeterMoveToToStandardPlan);
			if (!IsSmartAccount && (IsUpgradeToSmartAvailable || CanSmartMeterMoveToToStandardPlan))
			{
				var mcc = CanSmartMeterMoveToToStandardPlan ? RegisterConfigType.MCC12 : RegisterConfigType.MCC01;
				electricAccountConfig.WithElectricity24HrsDevices(mcc, CommsTechnicallyFeasibleValue.CTF3);
				//MCC12
			}
			UserConfig.Execute();

			SetupMocks();

			var app = await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMeterReading();
			app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToPlanPage();
			Sut = app.CurrentPageAs<PlanPage>();
		}

		[Test]
		public async Task CanSeePricePlanComponents()
		{
			var account = UserConfig.Accounts.First();
			Assert.IsTrue(Sut.PricePlanHeading?.TextContent.Equals("Your plan"));
			Assert.IsTrue(Sut.PricePlanLabel?.TextContent.Equals("Current price plan"));
			Assert.IsTrue(Sut.PricePlan?.TextContent.Equals(account.PlanName));

			Assert.IsTrue(Sut.PricePlanDiscountLabel?.TextContent.Equals("Discount"));
			Assert.IsTrue(Sut.PricePlanDiscount?.TextContent.Equals($"{account.DiscountAppliedPercentage}%"));

			if (!CanAddGasAccount || account.IsSmart())
			{
				Assert.IsNull(Sut.AddGas);
				Assert.IsNull(Sut.AddGasFlow);
			}

			if (account.IsSmart() || !account.IsElectricityAccount() || !account.IsOpen)
			{
				Assert.IsNull(Sut.UpgradeToSmartHeading);
				Assert.IsNull(Sut.UpgradeToSmartText);
				Assert.IsNull(Sut.UpgradeToSmartLink);
			}
		}

		[Test]
		public async Task CanSeeAccountBillingComponents()
		{
			var account = UserConfig.Accounts.First();

			Assert.IsTrue(Sut.AccountBillingHeading?.TextContent.Equals("Billing"));
			Assert.IsTrue(Sut.PaymentMethodLabel?.TextContent.Equals("Payment method"));
			Assert.IsTrue(Sut.PaymentMethod?.TextContent.Equals(account.PaymentMethod.ToUserText()));

			
			if (account.PaymentMethod.IsOneOf(PaymentMethodType.DirectDebit, PaymentMethodType.Equalizer))
			{
				Assert.IsTrue(Sut.DirectDebitBankLabel?.TextContent.Equals("Name on bank account"));
				Assert.IsTrue(Sut.DirectDebitBank?.TextContent.Equals(NameInBankAccount));
				Assert.IsTrue(Sut.DirectDebitIbanLabel?.TextContent.Equals("IBAN"));
				Assert.IsTrue(Sut.DirectDebitIban?.TextContent.Equals(Iban.Mask('*', Iban.Length - 4)));
			}
			else
			{
				Assert.IsNull(Sut.DirectDebitBankLabel);
				Assert.IsNull(Sut.DirectDebitBank);
				Assert.IsNull(Sut.DirectDebitIbanLabel);
				Assert.IsNull(Sut.DirectDebitIban);
			}

			await AssertDirectDebitLink();


			if (!CanSmartMeterMoveToToStandardPlan || IsSmartMeterOnChangeToStandardPlan)
			{
				Assert.IsNull(Sut.SmartMeterDataHeading);
				Assert.IsNull(Sut.SmartMeterDataToggle);
				Assert.IsNull(Sut.SmartMeterDataText);
				Assert.IsNull(Sut.SmartMeterDataDowngradCheckBoxText);
			}

			if (!account.IsElectricityAccount() ||
				!account.IsOpen ||
				!account.IsSmart())
			{
				Assert.IsNull(Sut.MonthlyBillingHeading);
				Assert.IsNull(Sut.MonthlyBillingToggle);

				Assert.IsNull(Sut.MonthlyBillChangeModal);
				Assert.IsNull(Sut.MonthlyBillChangeModalHeading);
				Assert.IsNull(Sut.MonthlyBillChangeModalLabel);
				Assert.IsNull(Sut.MonthlyBillChangeModalText);
				Assert.IsNull(Sut.MonthlyBillChangeModalDates);
				Assert.IsNull(Sut.MonthlyBillChangeModalContinue);
				Assert.IsNull(Sut.MonthlyBillChangeModalCancel);

				Assert.IsNull(Sut.CancelMonthlyBillModal);
				Assert.IsNull(Sut.CancelMonthlyBillModalHeading);
				Assert.IsNull(Sut.CancelMonthlyBillModalText);
				Assert.IsNull(Sut.CancelMonthlyBillModalContinue);
				Assert.IsNull(Sut.CancelMonthlyBillModalCancel);
			}

			if (!IsMonthlyBillingActive)
			{
				Assert.IsNull(Sut.MonthlyBillText);
				if (IsSmartAccount)
				{
					Assert.AreEqual("Monthly billing is not active. Your bill is currently issued every two months.",
						Sut.BiMonthlyBillText?.TextContent);
				}
			}

			if (!CanSwitchToMonthlyBilling)
			{
				Assert.IsNull(Sut.MonthlyBillChangeDate);
			}

			if (IsSmartAccount)
			{
				Assert.IsNull(Sut.EqualiserHeading);
				Assert.IsNull(Sut.EqualiserText);
				Assert.IsNull(Sut.EqualiserLink);
			}

			async Task AssertDirectDebitLink()
			{
				if (account.PaymentMethod.HasDirectDebitAvailable())
				{
					var directDebitButtonLabel =
						account.PaymentMethod.IsOneOf(PaymentMethodType.DirectDebit, PaymentMethodType.Equalizer)
							? "Edit Direct Debit"
							: "Set Up Direct Debit";

					if (!WithIsContractPending)
					{
						Assert.IsNotNull(Sut.EditDirectDebitLink);
						Assert.IsTrue(Sut.EditDirectDebitLink.TextContent.Equals(directDebitButtonLabel));
						var page = await App.ClickOnElement(Sut.EditDirectDebitLink);
						page.CurrentPageAs<InputDirectDebitDetailsPage>();
					}
					else
					{
						Assert.IsNull(Sut.EditDirectDebitLink);
					}
				}
				else
				{
					Assert.IsNull(Sut.EditDirectDebitLink);
				}
			}
		}
	}
}
