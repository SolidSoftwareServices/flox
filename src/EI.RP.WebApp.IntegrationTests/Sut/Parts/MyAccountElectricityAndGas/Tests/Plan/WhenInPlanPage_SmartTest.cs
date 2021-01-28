using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EI.RP.DomainServices.Commands.Billing.ChangeBillingPeriod;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Plan
{
    internal class WhenInPlanPage_SmartTest : WhenInPlanPageTests
    {
	    protected override bool IsSmartAccount => true;
		
		private const int MaxDateAllowedForBilling = 28;
		
		[Test]
	    public async Task CanSeeMonthlyBillingComponents()
	    {
			Assert.IsTrue(Sut.MonthlyBillingHeading?.TextContent.Equals("Monthly billing"));
			Assert.IsNotNull(Sut.MonthlyBillingToggle);

			Assert.IsNotNull(Sut.MonthlyBillChangeModal);
			Assert.IsTrue(Sut.MonthlyBillChangeModalHeading?.TextContent.Equals("What date would you like your bill on?"));
			Assert.IsTrue(Sut.MonthlyBillChangeModalLabel?.TextContent.Equals("Day of the month"));
			Assert.IsTrue(Sut.MonthlyBillChangeModalText?.TextContent.Equals("Payment will be due approximately 14 days after this date"));
			Assert.IsNotNull(Sut.MonthlyBillChangeModalDates);
			Assert.IsTrue(Sut.MonthlyBillChangeModalContinue?.TextContent.Trim().Equals("Continue"));
			Assert.IsTrue(Sut.MonthlyBillChangeModalCancel?.TextContent.Equals("Cancel"));

			Assert.IsNotNull(Sut.CancelMonthlyBillModal);
			Assert.IsTrue(Sut.CancelMonthlyBillModalHeading?.TextContent.Equals("Are you sure you want to cancel monthly billing?"));
			Assert.IsTrue(Sut.CancelMonthlyBillModalText?.TextContent.Equals("Your electricity bill will be issued every two months, and you'll no longer be able to select a billing date of your choice."));
			Assert.IsTrue(Sut.CancelMonthlyBillModalContinue?.TextContent.Trim().Equals("Continue"));
			Assert.IsTrue(Sut.CancelMonthlyBillModalCancel?.TextContent.Equals("Cancel"));

		}

		[Test]
	    public async Task SetMonthlyBillingDate()
	    {
		    var date = (Fixture.Create<int>() % MaxDateAllowedForBilling);
		    Sut.MonthlyBillChangeModalDates.Value = date.ToString();

		    var cmd = new SetMonthlyBillingPeriodCommand(UserConfig.Accounts.First().AccountNumber, date);
		    UserConfig.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd);
		    var page = await App.ClickOnElement(Sut.MonthlyBillChangeModalContinue);
		    page.CurrentPageAs<PlanPage>();
		    UserConfig.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);
		}

	    [Test]
	    public async Task CancelMonthlyBillingDate()
	    {
		    var cmd = new SetBiMonthlyBillingPeriodCommand(UserConfig.Accounts.First().AccountNumber);
		    UserConfig.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd);
		    var page = await App.ClickOnElement(Sut.CancelMonthlyBillModalContinue);
		    page.CurrentPageAs<PlanPage>();
		    UserConfig.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);
		}
	}
}
