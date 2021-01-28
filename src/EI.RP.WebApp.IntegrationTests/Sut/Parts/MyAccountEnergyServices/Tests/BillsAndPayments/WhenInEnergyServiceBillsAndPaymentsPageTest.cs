using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages.BillsAndPayments;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Tests.CommonHeaders;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Tests.BillsAndPayments
{
    [TestFixture]
    internal abstract class WhenInEnergyServiceBillsAndPaymentsPageTest : MyAccountEnergyServicesCommonTests<EnergyServiceBillsAndPaymentsPage>
    {
	    protected override async Task TestScenarioArrangement()
	    {
		    UserConfig = App.ConfigureUser("a@A.com", "test");

		    UserConfig.AddEnergyServicesAccount();
		    UserConfig.AddEnergyServicesAccount();
		    UserConfig.Execute();

		    var accountSelectionPage =
			    ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role))
			    .CurrentPageAs<AccountSelectionPage>();
		    await accountSelectionPage.SelectAccount(UserConfig.Accounts.First().AccountNumber);
		    await App.CurrentPageAs<MyAccountEnergyServicesPage>().ToBillsAndPayments();

		    Sut = App.CurrentPageAs<EnergyServiceBillsAndPaymentsPage>();
	    }

	    [Test]
        public abstract Task CanSeeComponentItems();
    }
}
