using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Billing.ChangeBillingPeriod;
using EI.RP.DomainServices.Commands.Billing.ChangePaperBillChoice;
using EI.RP.WebApp.Infrastructure.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Plan
{
	internal class WhenInPlanPage_SmartTest_MonthlyBill : WhenInPlanPage_SmartTest
	{
		protected override bool IsMonthlyBillingActive => true;
		protected override bool CanSwitchToMonthlyBilling => true;
		protected override int? MonthlyBillingDayOfMonth => (Fixture.Create<int>() % 28) + 1;

		public void CanSeeComponents()
		{
			Assert.IsTrue(Sut.MonthlyBillText?.TextContent.Equals($"Your bill is issued on the {MonthlyBillingDayOfMonth.ToOrdinal()} of every month. Payment will be due approximately 14 days after this date."));
			Assert.IsTrue(Sut.MonthlyBillChangeDate?.TextContent.Equals("Change date"));
			Assert.IsNull(Sut.BiMonthlyBillText);
		}

		[Test]
		public void ValidMessageForSwitchToPaperlessBill()
		{
			Assert.IsNotNull(Sut.PaperBillingConfirmationDialog);
			Assert.IsTrue(Sut.PaperBillingConfirmationDialogHeading?.TextContent.Trim().Equals("Are you sure?"));
			Assert.IsTrue(Sut.PaperBillingConfirmationDialogText?.TextContent.Contains("Your electricity bill will also be issued every two months instead of monthly, and you'll no longer be able to select a billing date of your choice."));
			Assert.IsTrue(Sut.PaperBillingConfirmationDialogYesButton?.TextContent.Trim().Equals("Yes I'm sure, continue"));
			Assert.IsTrue(Sut.PaperBillingConfirmationDialogNoButton?.TextContent.Trim().Equals("No, keep my savings"));
		}

		[Test]
		public async Task SwitchToPaperBill()
		{
			Assert.IsNotNull(Sut.PaperBillingConfirmationDialog);

			Assert.IsNotNull(Sut.PaperBillingConfirmationDialogYesButton);

			var cmd = new ChangePaperBillChoiceCommand(UserConfig.Accounts.First().AccountNumber, PaperBillChoice.Off);
			UserConfig.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd);

			var disableMonthlyBill = new SetBiMonthlyBillingPeriodCommand(UserConfig.Accounts.First().AccountNumber);
			UserConfig.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(disableMonthlyBill);

			Sut.PaperlessBillingToggle.IsChecked = false;
			var page = (await App.ClickOnElement(Sut.PaperBillingConfirmationDialogYesButton)).CurrentPageAs<PlanPage>();
			UserConfig.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);
			UserConfig.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(disableMonthlyBill);
		}
	}
}
