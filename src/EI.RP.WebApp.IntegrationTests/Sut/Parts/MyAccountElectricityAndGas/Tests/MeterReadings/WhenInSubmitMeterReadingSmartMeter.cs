using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Queries.Metering.MeterReadings;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.MeterReadings
{
	[TestFixture]
    abstract class WhenInSubmitMeterReadingSmartMeter : MyAccountCommonTests<SubmitMeterReadingPage_SmartMeter>
	{
        protected abstract CommsTechnicallyFeasibleValue CTF { get; }
        protected abstract RegisterConfigType MccConfiguration { get; }
        protected abstract bool HasMeterHistoryAvailable { get; }

        protected override async Task TestScenarioArrangement()
        {
	        UserConfig = App.ConfigureUser("a@A.com", "test");
	        UserConfig.AddElectricityAccount(configureDefaultDevice: false)
		        .WithElectricity24HrsDevices(MccConfiguration, CTF);
	        UserConfig.Execute();

	        if (HasMeterHistoryAvailable)
	        {
		        var meterReadingResults = Fixture.Build<MeterReadingInfo>()
			        .With(x => x.MeterType, MeterType.Electricity24h)
			        .Create()
			        .ToOneItemArray();

		        var electricityAccount = UserConfig.ElectricityAccounts().First();
		        App.DomainFacade.QueryResolver.ExpectQuery(new MeterReadingsQuery
		        {
			        AccountNumber = electricityAccount.Model.AccountNumber
		        }, meterReadingResults);


			}

			var withValidSessionFor = await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
	        var a = await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMeterReading();
	        Sut = a.CurrentPageAs<SubmitMeterReadingPage_SmartMeter>();
        }

        [Test]
        public async Task SubmitMeterReadingNotVisible()
        {
	        Assert.Null(Sut.MeterReadingSection);
        }
	}
}
