using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.MeterReadings
{
    [TestFixture]
    internal class WhenInSubmitMeterReading_Electricity24Hrs_NoMeterReadingHistory_Tests : WhenInSubmitMeterReadingTests<SubmitMeterReadingsPage_MeterInput1>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig
                .AddElectricityAccount(configureDefaultDevice: false)
                .WithElectricity24HrsDevices();

            UserConfig.Execute();
            var electricityAccount = UserConfig.ElectricityAccounts().First();

            var withValidSessionFor = await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMeterReading();
            Sut = App.CurrentPageAs<SubmitMeterReadingsPage_MeterInput1>();
        }

        [Test]
        public override async Task CanSubmit()
        {
            var inputs = CreateMeterInputs(1);
            Sut.MeterReadingInput1.Value = inputs[0].ToString();

            var meterReadingDataResults = CreateReadingDataResults(inputs, UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices);
            await CanSubmitMetersWithCorrectValuesCheckAsserts(UserConfig.Accounts.Single().AccountNumber,
                                                               ClientAccountType.Electricity,
                                                               meterReadingDataResults);
        }

        [Test]
        public async Task CanSeeComponents()
        {
            Assert.IsTrue(Sut.MeterReadingHistoryTable == null);
            Assert.IsTrue(Sut.MeterReadingHistoryTableMobile == null);
            Assert.IsTrue(Sut.MeterReadingH2Header == null);
        }
    }
}