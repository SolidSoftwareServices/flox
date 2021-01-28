using System;
using System.Collections.Generic;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Commands.SmartActivation.ActivateSmartMeter;
using EI.RP.WebApp.DomainModelExtensions;
using EI.RP.WebApp.Infrastructure.Extensions;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step5
{
	[TestFixture]
	abstract class WhenInSmartActivationStep5Test : WhenInSmartActivationTest<Step5SummaryPage>
	{
		protected abstract BillingFrequencyType BillingFrequency { get; }
		protected virtual int BillingDayOfMonth => 0;
		protected virtual DayOfWeek? SelectedFreeDay => null;

		protected SmartPlan SelectedPlan { get; set; }

		protected override async Task<Step5SummaryPage> NavigateToCurrentStep()
		{
			var step1Page = App.CurrentPageAs<Step1EnableSmartFeaturesPage>();
			var step2Page = (await step1Page.NextPage(true)).CurrentPageAs<Step2SelectPlanPage>();
			var step3Page = await NavigateToStep3(step2Page);
			var step4Page = await NavigateToStep4(step3Page);
			return await NavigateToStep5(step4Page);
		}

		protected virtual async Task<Step3PaymentDetailsPage> NavigateToStep3(Step2SelectPlanPage step2)
		{
			SelectedPlan = IsDualFuel ? UserConfig.ElectricityAndGasAccountConfigurators.First().SmartPlans[0] : UserConfig.ElectricityAndGasAccountConfigurators.Single().SmartPlans[0];
			await step2.SelectPlan(SelectedPlan);
			return App.CurrentPageAs<Step3PaymentDetailsPage>();
		}

		protected virtual async Task<Step4BillingFrequencyPage> NavigateToStep4(Step3PaymentDetailsPage step3)
		{
			if (PaymentMethod == PaymentMethodType.DirectDebitNotAvailable)
			{
				return (await step3.ClickOnElement(step3.AlternativePayerElement.ContinueButton)).CurrentPageAs<Step4BillingFrequencyPage>();
			}

			if (PaymentMethod == PaymentMethodType.DirectDebit)
			{
				step3.ExistingDirectDebitElement.ConfirmUseExistingDebit.IsChecked = true;
				return (await step3.ClickOnElement(step3.ExistingDirectDebitElement.UseExistingDirectDebitButton)).CurrentPageAs<Step4BillingFrequencyPage>();
			}

			if (PaymentMethod == PaymentMethodType.Manual)
			{
				var step4 = (await step3.ClickOnElement(step3.SetupDirectDebitElementForManualPayment.SkipButton));
				return step4.CurrentPageAs<Step4BillingFrequencyPage>();
			}

			throw new Exception($"Invalid {nameof(PaymentMethod)}");
		}

		protected virtual async Task<Step5SummaryPage> NavigateToStep5(Step4BillingFrequencyPage step4)
		{
			return (await step4.NextPage(BillingFrequency, BillingDayOfMonth)).CurrentPageAs<Step5SummaryPage>();
		}

		[Test]
		public override async Task CanSeeComponents()
		{
			Assert.IsNotNull(Sut.PageHeading);
			Assert.IsNotNull(Sut.SelectedPlan);
			Assert.IsNotNull(Sut.ContinueButton);
			Assert.AreEqual(SelectedPlan.PlanName, Sut.SelectedPlan?.TextContent);
			Assert.AreEqual(GetExpectedPaymentMethod(), Sut.PaymentMethod?.TextContent);
			Assert.AreEqual(GetBillFrequencyText(), Sut.BillFrequency?.TextContent);
		}

		[Test]
		public virtual async Task CanSeeValidationError()
		{
			var page = (await Sut.NextPage(false)).CurrentPageAs<Step5SummaryPage>();
			Assert.AreEqual("You need to check the box above before you can continue", page.TermsAndConditionsAcceptedCheckboxError?.TextContent);
		}

		[Test]
		public virtual async Task CanContinue()
		{
			var account = UserConfig.Accounts.First();
			(await Sut.NextPage(true)).CurrentPageAs<ConfirmationPage>();

			var cmd = new ActivateSmartMeterCommand(account.PointReferenceNumber.ToString(),
				account.AccountNumber,
				SelectedPlan,
				SelectedFreeDay,
				BillingFrequency == BillingFrequencyType.EveryMonth,
				BillingDayOfMonth,
				GetDirectDebitCommands());
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);
		}

		[Test]
		public virtual async Task CanSeeErrorPage()
		{
			var account = UserConfig.Accounts.First();
			var cmd = new ActivateSmartMeterCommand(account.PointReferenceNumber.ToString(),
				account.AccountNumber,
				SelectedPlan,
				SelectedFreeDay,
				BillingFrequency == BillingFrequencyType.EveryMonth,
				BillingDayOfMonth,
				GetDirectDebitCommands());
			App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(cmd, new DomainException(DomainError.Undefined));
			(await Sut.NextPage(true)).CurrentPageAs<SmartActivationGenericErrorPage>();
		}

		protected virtual List<SetUpDirectDebitDomainCommand> GetDirectDebitCommands() => new List<SetUpDirectDebitDomainCommand>();

		protected virtual string GetExpectedPaymentMethod()
		{
			return PaymentMethod == PaymentMethodType.DirectDebitNotAvailable
				? "Alternative Payer"
				: PaymentMethod.ToUserText();
		}

		private string GetBillFrequencyText()
		{
			switch (BillingFrequency)
			{
				case BillingFrequencyType.EveryTwoMonths:
					return "Every 2 months";
				case BillingFrequencyType.EveryMonth:
					return $"{BillingDayOfMonth.ToOrdinal()} of each month";
				default:
					throw new Exception($"Invalid {nameof(BillingFrequency)}");
			}
		}
	}
}
