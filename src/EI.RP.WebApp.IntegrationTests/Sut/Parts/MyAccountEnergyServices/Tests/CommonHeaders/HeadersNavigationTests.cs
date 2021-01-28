using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages.AccountOverview;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Tests.CommonHeaders
{
	[TestFixture]
	class HeadersNavigationTests: MyAccountEnergyServicesCommonTests<EnergyServicesAccountOverviewPage>
	{
		protected override async Task TestScenarioArrangement()
		{
			
			UserConfig = App.ConfigureUser("a@A.com", "test");

			UserConfig.AddEnergyServicesAccount();
			UserConfig.AddEnergyServicesAccount();
			UserConfig.Execute();

			var accountSelectionPage = ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role))
				.CurrentPageAs<AccountSelectionPage>();
			Sut = (await accountSelectionPage.SelectAccount(UserConfig.Accounts.First().AccountNumber))
				.CurrentPageAs<EnergyServicesAccountOverviewPage>();
		}

		[Test]
		public async Task CanLogout_NavigatesToLoginPage()
		{
			(await Sut.Logout()).CurrentPageAs<LoginPage>();
		}
	}
}