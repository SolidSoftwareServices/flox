using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Sut.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
    internal class WhenInShowPaymentsHistory_NoInvoicesTests : WhenInAccountPaymentsConfigurationTests
    {
	    private PlanPage _sut;
	    protected override async Task TestScenarioArrangement()
	    {
		    SetHasInvoices(false);
		    await base.TestScenarioArrangement();
	    }

	    [Test]
	    public async Task PrivacyLinkGoesToTermsInfoPage()
	    {
		    var sut = (await Sut.ClickOnElement(Sut.Overview.BillAndPaymentOptionsLink)).CurrentPageAs<PlanPage>();
		    (await sut.ToPrivacyNoticeViaFooter()).CurrentPageAs<TermsInfoPage>();
	    }

		[Test]
	    public async Task ShowUiForAccountWithNoBill()
	    {
		    Assert.IsNotNull(Sut.Overview.HowToReadLink);
			Assert.AreEqual("No payments have been applied to this account yet.", Sut.Overview.NoPaymentAppliedMessage?.TextContent);
			Assert.AreEqual("If you have recently made a payment, it can take up to 3 working days to be processed.", Sut.Overview.PaymentMessage?.TextContent);
	    }
    }
}