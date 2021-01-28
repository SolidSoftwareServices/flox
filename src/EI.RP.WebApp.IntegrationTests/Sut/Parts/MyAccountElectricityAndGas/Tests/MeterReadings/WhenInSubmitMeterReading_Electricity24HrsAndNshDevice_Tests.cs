using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainServices.Commands.Metering.SubmitMeterReading;
using EI.RP.DomainServices.Queries.Metering.MeterReadings;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.MeterReadings
{
	[TestFixture]
	internal class WhenInSubmitMeterReading_Electricity24HrsAndNshDevice_Tests : WhenInSubmitMeterReadingTests<SubmitMeterReadingsPage_MeterInput2>
	{
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount(configureDefaultDevice: false)
	            .Premise.WithDevicesForConfiguration(ConfigurableDeviceSet.Electricity24HAndNightStorageHeater);
            UserConfig.Execute();

            var meterReadingResultsElectricity24h = Fixture.Build<MeterReadingInfo>()
                .With(x => x.MeterType, MeterType.Electricity24h)
                .CreateMany(1)
                .ToArray();

            var meterReadingResultsElectricityNightStorageHeater = Fixture.Build<MeterReadingInfo>()
                .With(x => x.MeterType, MeterType.ElectricityNightStorageHeater)
                .CreateMany(1)
                .ToArray();

            var meterReadingResults = meterReadingResultsElectricity24h.Concat(meterReadingResultsElectricityNightStorageHeater);

            var electricityAccount = UserConfig.ElectricityAccounts().First();
            App.DomainFacade.QueryResolver.ExpectQuery(new MeterReadingsQuery
            {
                AccountNumber = electricityAccount.Model.AccountNumber
            }, meterReadingResults);

            var withValidSessionFor = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            var a = await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMeterReading();
            Sut = a.CurrentPageAs<SubmitMeterReadingsPage_MeterInput2>();
        }

        [Test]
        public override async Task CanSubmit()
        {
			var inputs = CreateMeterInputs(2);
			Sut.MeterReadingInput1.Value = inputs[0].ToString();
			Sut.MeterReadingInput2.Value = inputs[1].ToString();

			var meterReadingDataResults = CreateReadingDataResults(inputs, UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices);
			await CanSubmitMetersWithCorrectValuesCheckAsserts(UserConfig.Accounts.Single().AccountNumber,
															   ClientAccountType.Electricity,
															   meterReadingDataResults);
		}

        [Test]
        public async Task WhenEmptyShowsError()
        {
			var sut = Sut;
			sut.MeterReadingInput1.Value = string.Empty;
			sut.MeterReadingInput2.Value = string.Empty;
			sut = (await sut.ClickOnElement(sut.SubmitButton)).CurrentPageAs<SubmitMeterReadingsPage_MeterInput2>();

			Assert.IsTrue(sut.MeterReadingInput1.ClassList.Contains("input-validation-error"));
			Assert.IsTrue(sut.MeterReadingInput1.OuterHtml.Contains("Please enter a valid meter reading"));
			Assert.IsTrue(sut.MeterReadingInput1Error.InnerHtml.Contains("Please enter a valid meter reading"));

			Assert.IsTrue(sut.MeterReadingInput2.ClassList.Contains("input-validation-error"));
			Assert.IsTrue(sut.MeterReadingInput2.OuterHtml.Contains("Please enter a valid meter reading"));
			Assert.IsTrue(sut.MeterReadingInput2Error.InnerHtml.Contains("Please enter a valid meter reading"));
		}

        [Test]
        public async Task CanSeeComponents()
        {
            Assert.IsTrue(Sut.MeterReadingHistoryTable != null
                && !string.IsNullOrWhiteSpace(Sut.MeterReadingHistoryTableFirstReading.TextContent));

            Assert.IsTrue(Sut.MeterReadingHistoryTableMobile != null
                && !string.IsNullOrWhiteSpace(Sut.MeterReadingHistoryTableMobileFirstReading.TextContent));

            Assert.IsTrue(Sut.MeterReadingHistoryTable.InnerHtml.Contains(MeterType.Electricity24h.ToString()));
            Assert.IsTrue(Sut.MeterReadingHistoryTable.InnerHtml.Contains(MeterType.ElectricityNightStorageHeater.ToString()));

            Assert.IsTrue(Sut.MeterReadingHistoryTableMobile.InnerHtml.Contains(MeterType.Electricity24h.ToString()));
            Assert.IsTrue(Sut.MeterReadingHistoryTableMobile.InnerHtml.Contains(MeterType.ElectricityNightStorageHeater.ToString()));

			Assert.IsTrue(Sut.HowDoIReadMessageLink != null && Sut.HowDoIReadMessageLink.OuterHtml.Contains(MeterReadingTooltip24h));
		}

		[Test]
		public async Task WhenMeterReadingAlreadyReceived_ItShowsTheError()
		{
			var inputs = CreateMeterInputs(2);
			Sut.MeterReadingInput1.Value = inputs[0].ToString();
			Sut.MeterReadingInput2.Value = inputs[1].ToString();

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
			var inputs = CreateMeterInputs(2);
			Sut.MeterReadingInput1.Value = inputs[0].ToString();
			Sut.MeterReadingInput2.Value = inputs[1].ToString();
			var meterReadingDataResults = CreateReadingDataResults(inputs, UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices);
			await CannotSubmitMetersWhenLessThanActualNetworkCheckAsserts(UserConfig.Accounts.Single().AccountNumber,
																		  ClientAccountType.Electricity,
																		  meterReadingDataResults);
		}

		[Test]
		public async Task CannotSubmitMetersWhenLessThanActualCustomer()
		{
			var inputs = CreateMeterInputs(2);
			Sut.MeterReadingInput1.Value = inputs[0].ToString();
			Sut.MeterReadingInput2.Value = inputs[1].ToString();
			var meterReadingDataResults = CreateReadingDataResults(inputs, UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices);
			await CannotSubmitMetersWhenLessThanActualCustomerCheckAsserts(UserConfig.Accounts.Single().AccountNumber,
																		   ClientAccountType.Electricity,
																		   meterReadingDataResults);
		}
	}
}