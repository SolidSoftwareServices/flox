using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Commands.Metering.SubmitMeterReading;
using EI.RP.DomainServices.Queries.Contracts.BusinessPartners;
using EI.RP.DomainServices.Queries.Metering.MeterReadings;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Agent.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using AutoFixture;
using NUnit.Framework;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Usage;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Agent.Tests
{
    [TestFixture]
    class WhenInAgentBusinessPartnerSubmitMeterReadingTests : WebAppPageTests<SubmitMeterReadingsPage_MeterInput1>
    {
        public WhenInAgentBusinessPartnerSubmitMeterReadingTests() : base(ResidentialPortalDeploymentType.Internal)
        {
        }

        private AppUserConfigurator _esbAgentUser;
        private AppUserConfigurator _esbBusinessPartner;
        private readonly IFixture Fixture = new Fixture().CustomizeDomainTypeBuilders();

        protected override async Task TestScenarioArrangement()
        {
            _esbAgentUser = App.ConfigureUser("testagent@esb.ie", "Password$1", ResidentialPortalUserRole.AgentUser);
            _esbBusinessPartner = App.ConfigureUser("testBusinessPartner@esb.ie", "Password$1", ResidentialPortalUserRole.OnlineUser);
            _esbBusinessPartner.AddElectricityAccount();
            _esbBusinessPartner.Execute();
			var esbBusinessPartnerAccount = _esbBusinessPartner.Accounts.First();

			var businessPartner = new BusinessPartner
			{
				CommunicationsLevel = 0,
				Description = esbBusinessPartnerAccount.Description,
				MeterConfiguration = 0,
				NumPartner = esbBusinessPartnerAccount.AccountNumber
			};
			App.DomainFacade.QueryResolver.ExpectQuery(new BusinessPartnerQuery
			{
				UserName = _esbBusinessPartner.UserName,
				HouseNum = string.Empty,
				Street = string.Empty,
				City = string.Empty,
				PartnerNum = string.Empty,
                LastName = string.Empty
			}, new List<BusinessPartner>() {businessPartner});
            App.DomainFacade.QueryResolver.ExpectQuery(new BusinessPartnerQuery
	            {
		            UserName = null,
		            HouseNum = null,
		            Street = null,
		            City = null,
		            LastName = string.Empty,
		            PartnerNum = businessPartner.NumPartner,
		            NumberOfRows = 1
	            },
	            businessPartner.ToOneItemArray()
            );
            var queryResult = new List<AccountInfo>() { esbBusinessPartnerAccount };
            App.DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
			{
				AccountNumber = _esbBusinessPartner.Accounts.First().AccountNumber
			}, queryResult);

			App.DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
			{
				Opened = null
			}, queryResult);
			App.DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
			{
				BusinessPartner = _esbBusinessPartner.Accounts.First().AccountNumber
			}, queryResult);
			var electricityAccount = _esbBusinessPartner.ElectricityAccounts().First();
            var meterReadingResults = Fixture.Build<MeterReadingInfo>().CreateMany(3).ToArray();

            App.DomainFacade.QueryResolver.ExpectQuery(new MeterReadingsQuery
            {
                AccountNumber = electricityAccount.Model.AccountNumber
            }, meterReadingResults);

            var app = (ResidentialPortalApp) await App.WithValidSessionFor(_esbAgentUser.UserName, _esbAgentUser.Role);

            await app.ToAgentBusinessPartnerSearch();
            App.CurrentPageAs<AgentBusinessPartnerPage>().UserNameInput.Value = _esbBusinessPartner.UserName;
			await App.CurrentPageAs<AgentBusinessPartnerPage>().ClickOnElement(App.CurrentPageAs<AgentBusinessPartnerPage>().FindBusinessPartnersButton);
			await App.CurrentPageAs<AgentBusinessPartnerPage>().ClickOnElement(App.CurrentPageAs<AgentBusinessPartnerPage>().ViewBusinessPartnersButton);
            await App.CurrentPageAs<AccountSelectionPage>().SelectFirstAccount();
            await App.CurrentPageAs<UsagePage>().ToMeterReading();
            Sut = App.CurrentPageAs<SubmitMeterReadingsPage_MeterInput1>();
        }

        [Test]
        public async Task CanSubmit()
        {
			var inputs = new int[1] { Fixture.Create<int>() };
			Sut.MeterReadingInput1.Value = inputs[0].ToString();

			var meterReadingDataResults = CreateReadingDataResults(inputs, _esbBusinessPartner.ElectricityAndGasAccountConfigurators.Single().Premise.Devices);

			var cmd = new SubmitMeterReadingCommand(_esbBusinessPartner.Accounts.Single().AccountNumber, meterReadingDataResults, validateLastResults: true);

			App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd);
			var a = await Sut.ClickOnElement(Sut.SubmitButton);
			a.CurrentPageAs<SubmitMeterReadingsPage_SuccessfulSubmit>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);
		}

		private List<MeterReadingData> CreateReadingDataResults(int[] inputs, IEnumerable<DeviceInfo> devices)
		{
			var meterReadingDataResults = new List<MeterReadingData>();
			var inputIndex = 0;

			foreach (var device in devices)
			{
				foreach (var register in device.Registers)
				{
					var meterData = new MeterReadingData
					{
						DeviceId = register.DeviceId,
						MeterNumber = register.MeterNumber,
						MeterReading = inputs[inputIndex].ToString(),
						RegisterId = register.RegisterId,
						MeterTypeName = register.MeterType.ToString()
					};
					meterReadingDataResults.Add(meterData);
					inputIndex++;
				}
			}

			return meterReadingDataResults;
		}

		[Test]
        public async Task CanSeeComponents()
        {
            Assert.IsTrue(Sut.MeterReadingInput1 != null);

            Assert.IsTrue(Sut.MeterReadingHistoryTable != null
                && !string.IsNullOrWhiteSpace(Sut.MeterReadingHistoryTableFirstReading.TextContent));

            Assert.IsTrue(Sut.MeterReadingHistoryTableMobile != null
                && !string.IsNullOrWhiteSpace(Sut.MeterReadingHistoryTableMobileFirstReading.TextContent));
        }

        [Test]
        public async Task WhenMeterReadingUnableToProcessRequest_ItShowsExpectedErrorMessage()
        {
			var inputs = new int[1] { Fixture.Create<int>() };
			Sut.MeterReadingInput1.Value = inputs[0].ToString();

			var meterReadingDataResults = CreateReadingDataResults(inputs, _esbBusinessPartner.ElectricityAndGasAccountConfigurators.Single().Premise.Devices);

			var cmd = new SubmitMeterReadingCommand(_esbBusinessPartner.Accounts.Single().AccountNumber, meterReadingDataResults, validateLastResults: true);

			App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(cmd, new DomainException(ResidentialDomainError.UnableToProcessRequest));
			await Sut.ClickOnElement(Sut.SubmitButton);
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);
			var errorPage = App.CurrentPageAs<SubmitMeterReadingsPage_SubmitError>();

			Assert.IsTrue(errorPage.MeterReadingErrorTitle.TextContent.Contains("Sorry, we can’t take your reading right now"));
			Assert.IsTrue(errorPage.MeterReadingErrorMessage.TextContent.Contains("Sorry, Unable to process your request."));
		}

        [Test]
        public async Task WhenMeterReadingDataAlreadyReleased_ItShowsExpectedErrorMessage()
        {
			var inputs = new int[1] { Fixture.Create<int>() };
			Sut.MeterReadingInput1.Value = inputs[0].ToString();

			var meterReadingDataResults = CreateReadingDataResults(inputs, _esbBusinessPartner.ElectricityAndGasAccountConfigurators.Single().Premise.Devices);

			var cmd = new SubmitMeterReadingCommand(_esbBusinessPartner.Accounts.Single().AccountNumber, meterReadingDataResults, validateLastResults: true);

			App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(cmd, new DomainException(ResidentialDomainError.DataAlreadyReleased));
			await Sut.ClickOnElement(Sut.SubmitButton);
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);
			var errorPage = App.CurrentPageAs<SubmitMeterReadingsPage_SubmitError>();

			Assert.IsTrue(errorPage.MeterReadingErrorTitle.TextContent.Contains("Sorry, we can’t take your reading right now"));
			Assert.IsTrue(errorPage.MeterReadingErrorMessage.TextContent.Contains("Sorry, Unable to process your request."));
		}
    }
}