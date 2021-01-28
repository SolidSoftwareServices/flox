using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.PromotionEntry;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.PromotionEntry
{
	[TestFixture]
	internal class WhenInPagePromotionEntry : MyAccountCommonTests<PromotionEntryPage>
	{
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount()
				.WithInvoices(3);
			UserConfig.Execute();

			var app = await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToPromotionEntry();
			Sut = app.CurrentPageAs<PromotionEntryPage>();
		}

		[Test]
		public async Task CanSeeComponentItems()
		{
			Assert.IsNotNull(Sut.PromotionHeaderImage);
			Assert.IsNotNull(Sut.PromotionHeading);
			Assert.IsNotNull(Sut.PromotionDescription1);
			Assert.IsNotNull(Sut.PromotionDescription2);
			Assert.IsNotNull(Sut.PromotionDescription3);
			Assert.IsNotNull(Sut.PromotionDescription4);
			Assert.IsNotNull(Sut.PromotionLink?.Href);
			Assert.IsNotNull(Sut.PromotionTermsConditionLink?.Href);
		}

		[Test]
		public async Task CanSeeAllowedMenuItemsInPromotionSubPage()
		{
			Assert.NotNull(Sut.ChangePasswordProfileMenuItem);
			Assert.NotNull(Sut.LogoutProfileMenuItem);
		}

		[Test]
		public async Task CannotSeeForbiddenMenuItemsInPromotionSubPage()
		{
			Assert.Null(Sut.MyDetailsProfileMenuItem);
			Assert.Null(Sut.MarketingProfileMenuItem);
		}
	}
}
