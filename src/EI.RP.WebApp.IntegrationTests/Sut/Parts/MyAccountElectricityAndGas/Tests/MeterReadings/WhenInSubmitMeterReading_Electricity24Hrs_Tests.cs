using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DomainServices.Commands.Metering.SubmitMeterReading;
using EI.RP.DomainServices.Queries.Metering.MeterReadings;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.MeterReadings
{
    [TestFixture]
    internal class WhenInSubmitMeterReading_Electricity24Hrs_Tests : WhenInSubmitMeterReadingTests<SubmitMeterReadingsPage_MeterInput1>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig
                .AddElectricityAccount(configureDefaultDevice: false)
                .WithElectricity24HrsDevices();

            UserConfig.Execute();
            var electricityAccount = UserConfig.ElectricityAccounts().First();

            var meterReadingResults = Fixture.Build<MeterReadingInfo>()
                .With(x => x.MeterType, MeterType.Electricity24h)
                .CreateMany(3)
                .ToArray();

            App.DomainFacade.QueryResolver.ExpectQuery(new MeterReadingsQuery
            {
                AccountNumber = electricityAccount.Model.AccountNumber
            }, meterReadingResults);

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
            Assert.IsTrue(Sut.MeterReadingInput1 != null);
			Assert.IsTrue(Sut.MeterReadingInput1.GetAttribute("inputmode").Contains("numeric"));

			Assert.IsTrue(Sut.MeterReadingHistoryTable != null
                && !string.IsNullOrWhiteSpace(Sut.MeterReadingHistoryTableFirstReading.TextContent));

            Assert.IsTrue(Sut.MeterReadingHistoryTableMobile != null
                && !string.IsNullOrWhiteSpace(Sut.MeterReadingHistoryTableMobileFirstReading.TextContent));

            Assert.IsTrue(Sut.MeterReadingHistoryTable.InnerHtml.Contains(MeterType.Electricity24h.ToString()));
            Assert.IsTrue(Sut.MeterReadingHistoryTableMobile.InnerHtml.Contains(MeterType.Electricity24h.ToString()));
            Assert.IsTrue(Sut.HowDoIReadMessageLink != null && Sut.HowDoIReadMessageLink.OuterHtml.Contains(MeterReadingTooltip24h));
			Assert.AreEqual("https://www.electricireland.ie/residential/help/meters/how-to-read-your-electricity-meter", Sut.ForOtherMetersLink.Href);
        }

        [Test]
        public async Task WhenNoMeterReadingHistory_ShowsNoTable()
        {
            Assert.IsNotNull(Sut.MeterReadingHistoryTable);
            Assert.IsNotNull(Sut.MeterReadingH2Header);
        }

        [Test]
        public async Task WhenEmptyShowsError()
        {
            var sut = Sut;
            sut.MeterReadingInput1.Value = string.Empty;
            sut = (await sut.ClickOnElement(sut.SubmitButton)).CurrentPageAs<SubmitMeterReadingsPage_MeterInput1>();

            Assert.IsTrue(sut.MeterReadingInput1.ClassList.Contains("input-validation-error"));
            Assert.IsTrue(sut.MeterReadingInput1.OuterHtml.Contains("Please enter a valid meter reading"));
            Assert.IsTrue(sut.MeterReadingInput1Error.InnerHtml.Contains("Please enter a valid meter reading"));
        }

        [Test]
        public async Task WhenMeterReadingAlreadyReceived_ItShowsTheError()
        {
            var inputs = CreateMeterInputs(1);
            Sut.MeterReadingInput1.Value = inputs[0].ToString();

            var meterReadingDataResults = CreateReadingDataResults(inputs, UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices);

            var cmd = new SubmitMeterReadingCommand(UserConfig.Accounts.Single().AccountNumber, meterReadingDataResults, validateLastResults: true);
            App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(cmd, new DomainException(ResidentialDomainError.MeterReadingAlreadyReceived));
            var a = await Sut.ClickOnElement(Sut.SubmitButton);

            App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);
            var errorPage = App.CurrentPageAs<SubmitMeterReadingsPage_SubmitError>();

            Assert.IsTrue(errorPage.MeterReadingErrorTitle.TextContent.Contains("Sorry, we can’t take your reading right now"));
            Assert.IsTrue(errorPage.MeterReadingErrorMessage.TextContent.Contains(ResidentialDomainError.MeterReadingAlreadyReceived.ErrorMessage));
        }

        [Test]
        public async Task CannotSubmitMetersWhenLessThanActualNetwork()
        {
            var inputs = CreateMeterInputs(1);
            Sut.MeterReadingInput1.Value = inputs[0].ToString();
            var meterReadingDataResults = CreateReadingDataResults(inputs, UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices);
            await CannotSubmitMetersWhenLessThanActualNetworkCheckAsserts(UserConfig.Accounts.Single().AccountNumber,
                                                                          ClientAccountType.Electricity,
                                                                          meterReadingDataResults);
        }

        [Test]
        public async Task CannotSubmitMetersWhenLessThanActualCustomer()
        {
            var inputs = CreateMeterInputs(1);
            Sut.MeterReadingInput1.Value = inputs[0].ToString();
            var meterReadingDataResults = CreateReadingDataResults(inputs, UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices);
            await CannotSubmitMetersWhenLessThanActualCustomerCheckAsserts(UserConfig.Accounts.Single().AccountNumber,
                                                                           ClientAccountType.Electricity,
                                                                           meterReadingDataResults);
        }
    }
}