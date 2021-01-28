using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Queries.Metering.MeterReadings;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.MeterReadings
{
	internal class WhenInSubmitMeterReadingSmartMeter_MCC12 : MyAccountCommonTests<SubmitMeterReadingNotAvailablePage>
	{
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricity24HrsDevices(RegisterConfigType.MCC12, CommsTechnicallyFeasibleValue.CTF3);
			UserConfig.Execute();

			var meterReadingResults = Fixture.Build<MeterReadingInfo>()
				.With(x => x.MeterType, MeterType.Electricity24h)
				.Create()
				.ToOneItemArray();

			var electricityAccount = UserConfig.ElectricityAccounts().First();
			App.DomainFacade.QueryResolver.ExpectQuery(new MeterReadingsQuery
			{
				AccountNumber = electricityAccount.Model.AccountNumber
			}, meterReadingResults);

			var withValidSessionFor = await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			var a = await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMeterReading();
			Sut = a.CurrentPageAs<SubmitMeterReadingNotAvailablePage>();
		}

		[Test]
		public async Task SubmitMeterReadingNotVisible()
		{
			Assert.IsNotNull(Sut.Heading);
		}
	}
}