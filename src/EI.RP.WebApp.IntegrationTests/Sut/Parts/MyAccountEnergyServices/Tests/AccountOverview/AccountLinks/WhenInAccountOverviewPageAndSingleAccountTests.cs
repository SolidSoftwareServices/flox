using System.Linq;
using System.Threading.Tasks;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages.AccountOverview;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Tests.AccountOverview.AccountLinks
{


	[TestFixture]
	class WhenInAccountOverviewPageAndSingleAccountTests : WebAppPageTests<EnergyServicesAccountOverviewPage>
	{

		protected override async Task TestScenarioArrangement()
		{
		
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddEnergyServicesAccount();
			UserConfig.Execute();
			
            await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();

            Sut = App.CurrentPageAs<EnergyServicesAccountOverviewPage>();
        }

		public AppUserConfigurator UserConfig { get; set; }
	}
}