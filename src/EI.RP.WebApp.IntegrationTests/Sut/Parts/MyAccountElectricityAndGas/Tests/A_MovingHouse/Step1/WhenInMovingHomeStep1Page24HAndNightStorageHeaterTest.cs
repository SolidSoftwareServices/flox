using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step1
{
	[TestFixture]
	class WhenInMovingHomeStep1Page24HAndNightStorageHeaterTest : MyAccountCommonTests<Step1InputMoveOutPage>
	{
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricity24HrsDevices()
				.WithElectricityNightStorageHeaterDevice();
			UserConfig.Execute();
			var withValidSessionFor = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			var movingHomeLandingPage = (await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome()).CurrentPageAs<Step0LandingPage>();

			Sut = (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton1))
				.CurrentPageAs<Step1InputMoveOutPage>();
		}

		[Test]
		public async Task CanSeeComponents()
		{
			var registers = UserConfig.ElectricityAccounts().Single().Premise.Devices.SelectMany(x=>x.Registers).ToArray();
			Assert.IsNotNull(Sut.GetElecReadingInput(registers.Single(x => x.MeterType == MeterType.ElectricityNightStorageHeater).MeterNumber), "Expected to see NSH input");
			Assert.IsNotNull(Sut.GetElecReadingInput(registers.Single(x => x.MeterType == MeterType.Electricity24h).MeterNumber), "Expected to see 24H electricity input");

		}
	}
}