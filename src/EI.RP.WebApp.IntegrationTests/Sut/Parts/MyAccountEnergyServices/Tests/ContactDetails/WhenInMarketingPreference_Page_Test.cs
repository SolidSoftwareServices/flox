using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages.AccountOverview;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages.ContactDetails;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Tests.CommonHeaders;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Tests.ContactDetails
{
	internal class WhenInMarketingPreference_Page_Test : MyAccountEnergyServicesCommonTests<MyAccountEnergyServicesPage>
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

		    await sut.ToMarketingPreference();
		    Sut = App.CurrentPageAs<MarketingPreferencePage>();
	    }

	    [Test]
	    public async Task CanSeeComponentItems()
	    {
		    Assert.AreEqual("Marketing Preferences", Sut.MarketingPreference.MarketingPreferenceHeader);
		    Assert.AreEqual("Yes! I would like to receive updates about products & services, special offers, energy saving tips and news & events from Electric Ireland via:", Sut.MarketingPreference.MarketingPreferenceTopMessage);
		    Assert.AreEqual("You can unsubscribe at any time by adjusting your preferences above and clicking on \"Save Changes\" or by speaking to a customer service agent.", Sut.MarketingPreference.MarketingPreferenceBottomMessage);
	    }

	}
}
