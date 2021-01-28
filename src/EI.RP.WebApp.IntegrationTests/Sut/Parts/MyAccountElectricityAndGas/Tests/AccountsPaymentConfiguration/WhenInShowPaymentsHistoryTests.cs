using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using EI.RP.WebApp.IntegrationTests.Sut.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
	[TestFixture]
	internal class WhenInShowPaymentsHistoryTests : WhenInAccountPaymentsConfigurationTests
	{
		private PlanPage _sut;
		protected override async Task TestScenarioArrangement()
		{
			await base.TestScenarioArrangement();
			_sut = (await Sut.ClickOnElement(Sut.Overview.BillAndPaymentOptionsLink)).CurrentPageAs<PlanPage>();
		}

        [Test]
        public async Task PrivacyLinkGoesToTermsInfoPage()
        {
            (await _sut.ToPrivacyNoticeViaFooter()).CurrentPageAs<TermsInfoPage>();
        }
    }
}