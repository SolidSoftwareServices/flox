using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Billing.ChangePaperBillChoice;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Plan
{
	internal class WhenHasPaperlessBillTests : WhenInPlanPageTests
    {
	    protected override bool IsSmartAccount => false;
	    protected override bool IsUpgradeToSmartAvailable => true;
	    protected override bool WithPaperBill => false;
	    
	    [Test]
	    public async Task SwitchToPaperBill()
	    {
		    Assert.IsNotNull(Sut.PaperBillingConfirmationDialog);
		    Assert.IsTrue(Sut.PaperBillingConfirmationDialogHeading?.TextContent.Trim().Equals("Are you sure?"));
		    Assert.IsTrue(Sut.PaperBillingConfirmationDialogText?.TextContent.Trim().Replace("\n","").Equals("By turning off Paperless Billing you will no longer receive the 0.5% saving. You can continue to login to Your Account Online, however your bills will be sent by post. Otherwise, your price plan terms and conditions will not change. Read T&Cs."));
		    Assert.IsTrue(Sut.PaperBillingConfirmationDialogYesButton?.TextContent.Trim().Equals("Yes I'm sure, continue"));
		    Assert.IsTrue(Sut.PaperBillingConfirmationDialogNoButton?.TextContent.Trim().Equals("No, keep my savings"));

		    var cmd = new ChangePaperBillChoiceCommand(UserConfig.Accounts.First().AccountNumber, PaperBillChoice.Off);
		    UserConfig.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd);
		    //TODO: JS not supported in the client
		    var page = await App.ClickOnElement(Sut.PaperBillingConfirmationDialogYesButton);
		    page.CurrentPageAs<PlanPage>();
		    UserConfig.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);
		}
		
		[Test]
		public async Task PaperlessBillingOptionAvailable()
		{
			Assert.IsTrue(Sut.PaperlessBillingHeading?.TextContent.Equals("Paperless billing"));
			Assert.IsNotNull(Sut.PaperlessBillingToggle);

			var paperlessBillingText = Sut.PaperlessBillingText?.TextContent.Trim();

			Assert.IsTrue(paperlessBillingText?.StartsWith("Paperless billing adds 0.5% discount on your energy prices. Bills are delivered by email and are accessible in the"));
			Assert.IsTrue(paperlessBillingText?.EndsWith("payments section"));

			Assert.AreEqual(!WithPaperBill, Sut.PaperlessBillSelected);
			Assert.AreEqual(WithPaperBill,Sut.PaperBillSelected);
			//TODO: JS not supported
			//var page = await App.ClickOnElement(Sut.PaperlessBillingLink);
			//page.CurrentPageAs<ShowPaymentsHistoryPage>();

			//Assert.IsFalse(Sut.PaperlessBillSelected);
			//Assert.IsTrue(Sut.PaperBillSelected);
		}
	}
}
