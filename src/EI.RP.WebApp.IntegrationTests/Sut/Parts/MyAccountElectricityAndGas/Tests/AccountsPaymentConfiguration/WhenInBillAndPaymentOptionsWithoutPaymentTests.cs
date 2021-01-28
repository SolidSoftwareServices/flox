using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
	[TestFixture]
	internal class WhenInBillAndPaymentOptionsWithoutPaymentTests : WhenInAccountPaymentsConfigurationTests
	{
		protected override async Task TestScenarioArrangement()
		{
            async Task AUserIsInTheAccountsPage()
            {

                UserConfig = App.ConfigureUser("a@A.com", "test");
                UserConfig.AddElectricityAccount(withPaperBill: HasPaperBill);
                UserConfig.Execute();

                await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();

                Sut = (await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToBillingAndPayments()).CurrentPageAs<ShowPaymentsHistoryPage>();
            }

            await AUserIsInTheAccountsPage();

            Sut = (await Sut.ClickOnElement(Sut.Overview.BillAndPaymentOptionsLink)).CurrentPageAs<ShowPaymentsHistoryPage>();
		}
    }
}