using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Billing.ChangePaperBillChoice;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Plan
{
	internal class WhenPaperBillIsConfiguredTests:WhenInPlanPageTests
    {
	    protected override bool IsSmartAccount => false;
	    protected override bool IsUpgradeToSmartAvailable => true;
		protected override bool WithPaperBill => true;
		protected override bool WithIsContractPending => false;

		[Test]
		public async Task SwitchToPaperlessBill()
		{
			Assert.IsNotNull(Sut.PaperBillingConfirmationDialog);
			Assert.IsTrue(Sut.PaperBillingConfirmationDialogHeading?.TextContent.Trim().Equals("Are you sure?"));
			Assert.IsTrue(Sut.PaperBillingConfirmationDialogText?.TextContent.Trim().Replace("\n", "").Equals("By turning off Paperless Billing you will no longer receive the 0.5% saving. You can continue to login to Your Account Online, however your bills will be sent by post. Otherwise, your price plan terms and conditions will not change. Read T&Cs."));
			Assert.IsTrue(Sut.PaperBillingConfirmationDialogYesButton?.TextContent.Trim().Equals("Yes I'm sure, continue"));
			Assert.IsTrue(Sut.PaperBillingConfirmationDialogNoButton?.TextContent.Trim().Equals("No, keep my savings"));

			var cmd = new ChangePaperBillChoiceCommand(UserConfig.Accounts.First().AccountNumber, PaperBillChoice.On);
			UserConfig.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd);
			Sut.PaperlessBillingToggle.IsChecked = true;
			var page = await App.ClickOnElement(Sut.PaperlessBillingToggle);
			page.CurrentPageAs<PlanPage>();
			UserConfig.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);
		}

		[Test]
	    public async Task TheViewShowsTheExpectedInformation()
	    {
		    var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
			Assert.IsNotNull(Sut.PaperlessBillingLink);
			Assert.IsNull(Sut.PaperlessBillingDisabledText);
			Assert.IsFalse(Sut.PaperlessBillSelected);
			Assert.IsFalse(Sut.PaperlessBillingToggle.HasAttribute("disabled"));
		    Assert.IsTrue(Sut.PaperBillSelected);
		    Assert.AreEqual(PaperBillChoice.On, accountInfo.PaperBillChoice);
	    }
    }
}
