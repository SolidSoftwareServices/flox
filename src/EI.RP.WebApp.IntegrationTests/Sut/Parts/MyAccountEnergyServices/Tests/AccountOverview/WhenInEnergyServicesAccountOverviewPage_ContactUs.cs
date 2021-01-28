using NUnit.Framework;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Tests.AccountOverview
{
    [TestFixture]
	class WhenInEnergyServicesAccountOverviewPage_ContactUs : WhenInAccountOverviewEnergyServicesPageTests
	{
		[Test]
		public override async Task CanSeeComponentItems()
        {
            Assert.IsNotNull(Sut.ContactUs.Component);
        }

        [Test]
        public async Task ClickOnEmail()
        {
            Assert.IsNotNull(Sut.ContactUs.ContactUsLink);
        }
    }
}
