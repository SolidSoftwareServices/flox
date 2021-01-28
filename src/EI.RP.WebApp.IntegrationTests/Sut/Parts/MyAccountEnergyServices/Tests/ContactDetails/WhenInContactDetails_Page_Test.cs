using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages.AccountOverview;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages.ContactDetails;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Tests.ContactDetails
{
    [TestFixture]
    internal class WhenInContactDetails_Page_Test : MyAccountEnergyServicesCommonTests<MyAccountEnergyServicesPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");

            UserConfig.AddEnergyServicesAccount();
            UserConfig.AddEnergyServicesAccount();
            UserConfig.Execute();

            var accountSelectionPage = ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role))
                .CurrentPageAs<AccountSelectionPage>();
            var sut = (await accountSelectionPage.SelectAccount(UserConfig.Accounts.First().AccountNumber))
                .CurrentPageAs<EnergyServicesAccountOverviewPage>();

            await sut.ToContactDetails();
            Sut = App.CurrentPageAs<ContactDetailsPage>();
        }

        [Test]
        public  void CanSeeComponentItems()
        {
            Assert.AreEqual("Contact Details", Sut.ContactDetails.ContactDetailsHeader);
        }
    }

   
}
