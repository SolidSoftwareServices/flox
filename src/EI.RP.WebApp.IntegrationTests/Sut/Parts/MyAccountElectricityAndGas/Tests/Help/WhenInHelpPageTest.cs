using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Help;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Help
{
    [TestFixture]
    class WhenInHelpPageTest: MyAccountCommonTests<HelpPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount().WithInvoices(3);
            UserConfig.Execute();
            
            var app = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToHelp();
            Sut = app.CurrentPageAs<HelpPage>();
        }

        [Test]
        public async Task CanSeeComponentItems()
        {
            Assert.NotNull(Sut.BillingContainer);
            Assert.IsTrue(Sut.BillingTitle?.TextContent?.Equals("Billing"));
            Assert.IsTrue(Sut.BillingLink?.Href?.Equals("https://electricireland.ie/residential/help/billing"));

            Assert.NotNull(Sut.MovingHomeContainer);
            Assert.IsTrue(Sut.MovingHomeTitle?.TextContent?.Equals("Moving Home"));
            Assert.IsTrue(Sut.MovingHomeLink?.Href?.Equals("https://electricireland.ie/residential/help/moving-in-or-out"));

            Assert.NotNull(Sut.MetersContainer);
            Assert.IsTrue(Sut.MetersTitle?.TextContent?.Equals("Meters"));
            Assert.IsTrue(Sut.MetersLink?.Href?.Equals("https://electricireland.ie/residential/help/meters"));

            Assert.NotNull(Sut.EfficiencyContainer);
            Assert.IsTrue(Sut.EfficiencyTitle?.TextContent?.Equals("Efficiency"));
            Assert.IsTrue(Sut.EfficiencyLink?.Href?.Equals("https://electricireland.ie/residential/help/efficiency"));

            Assert.NotNull(Sut.SpecialNeedsContainer);
            Assert.IsTrue(Sut.SpecialNeedsTitle?.TextContent?.Equals("Customer with Special Needs"));
            Assert.IsTrue(Sut.SpecialNeedsLink?.Href?.Equals("https://electricireland.ie/residential/help/help-for-customers-with-special-needs"));

            Assert.NotNull(Sut.OnlineBillingContainer);
            Assert.IsTrue(Sut.OnlineBillingTitle?.TextContent?.Equals("Online Billing"));
            Assert.IsTrue(Sut.OnlineBillingLink?.Href?.Equals("https://electricireland.ie/residential/help/online-billing"));

            Assert.NotNull(Sut.MicroGenerationContainer);
            Assert.IsTrue(Sut.MicroGenerationTitle?.TextContent?.Equals("Micro Generation"));
            Assert.IsTrue(Sut.MicroGenerationLink?.Href?.Equals("https://electricireland.ie/residential/help/micro-generation"));

            Assert.NotNull(Sut.SafetyContainer);
            Assert.IsTrue(Sut.SafetyTitle?.TextContent?.Equals("Safety"));
            Assert.IsTrue(Sut.SafetyLink?.Href?.Equals("https://electricireland.ie/residential/help/safety"));

            Assert.NotNull(Sut.RewardsProgrammeContainer);
            Assert.IsTrue(Sut.RewardsProgrammeTitle?.TextContent?.Equals("Rewards Programme"));
            Assert.IsTrue(Sut.RewardsProgrammeLink?.Href?.Equals("https://electricireland.ie/residential/help/rewards-programme"));
        }

        [Test]
        public async Task CanSeeAllowedMenuItemsInHelpPage()
        {
            Assert.NotNull(Sut.ChangePasswordProfileMenuItem);
            Assert.NotNull(Sut.LogoutProfileMenuItem);
        }

        [Test]
        public async Task CannotSeeForbiddenMenuItemsInHelpPage()
        {
            Assert.Null(Sut.MyDetailsProfileMenuItem);
            Assert.Null(Sut.MarketingProfileMenuItem);
        }
    }
}
