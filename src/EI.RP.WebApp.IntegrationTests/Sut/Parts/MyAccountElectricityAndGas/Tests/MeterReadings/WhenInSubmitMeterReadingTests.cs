using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.Commands.Metering.SubmitMeterReading;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.MeterReadings
{
	[TestFixture]
	abstract class WhenInSubmitMeterReadingTests <TPage>: MyAccountCommonTests<TPage>
	where TPage:SubmitMeterReadingsPage
	{
		public const string MeterReadingTooltip24h = "#modalMeterReadingTooltip24h";
		public const string MeterReadingTooltipGas = "#modalMeterReadingTooltipGas";
		public const string MeterReadingTooltipDayNight = "#modalMeterReadingTooltipDayNight";

		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount();
			UserConfig.Execute();
            var withValidSessionFor = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            var a = await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMeterReading();
				Sut=a.CurrentPageAs<TPage>();
		}

		[Test]
		public abstract Task CanSubmit();

		[Test, Ignore("TODO")]
		public async Task CanReadMeterHistory()
		{
			throw new NotImplementedException();
		}

		[Ignore("TODO")]
		[Test]
		public async Task TODO_ON_ERROR_TESTS()
		{
			throw new NotImplementedException();
		}

		public int[] CreateMeterInputs(int size)
		{
			var fixture = new Fixture().CustomizeDomainTypeBuilders();
			var inputs = new int[size];

			for (var i = 0; i < size; i++)
				inputs[i] = fixture.Create<int>();

			return inputs;
		}	

		public List<MeterReadingData> CreateReadingDataResults(int[] inputs, IEnumerable<DeviceInfo> devices)
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

		public async Task CanSubmitMetersWithCorrectValuesCheckAsserts(string accountNumber,
																	   ClientAccountType accountType,
																	   List<MeterReadingData> meterReadings)
		{
			var cmd = new SubmitMeterReadingCommand(UserConfig.Accounts.Single().AccountNumber, meterReadings, validateLastResults: true);
			App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd);
			var a = await Sut.ClickOnElement(Sut.SubmitButton);

			a.CurrentPageAs<SubmitMeterReadingsPage_SuccessfulSubmit>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);
		}

		public async Task CannotSubmitMetersWhenLessThanActualNetworkCheckAsserts(string accountNumber, 
																				  ClientAccountType accountType,
																				  List<MeterReadingData> meterReadings)
		{
			var cmd = new SubmitMeterReadingCommand(accountNumber, meterReadings, validateLastResults: true);
			App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(cmd, new DomainException(ResidentialDomainError.MeterReadingLessThanActualNetwork));
			var a = await Sut.ClickOnElement(Sut.SubmitButton);

			var errorPage = a.CurrentPageAs<SubmitMeterReadingsPage_SubmitErrorLessThanActual>();			
			
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);

			Assert.IsTrue(errorPage.MeterReadingErrorTitle.TextContent.Contains("Sorry, we can’t take your reading right now"));

			if (accountType == ClientAccountType.Electricity)
				Assert.IsTrue(errorPage.MeterReadingErrorMessage.TextContent.Contains("Sorry unfortunately we cannot accept your reading as it is less than the last ESB networks provided actual read. If you are sure your reading is correct please send a dated photo of your meter to meterreadingchanges@electricireland.ie. Please ensure your MPRN is in the subject line of the email."));

			if (accountType == ClientAccountType.Gas)
				Assert.IsTrue(errorPage.MeterReadingErrorMessage.TextContent.Contains("Sorry unfortunately we cannot accept your reading as it is less than the last Gas Networks Ireland provided actual read. If you are sure your reading is correct please send a dated photo of your meter to meterreadingchanges@electricireland.ie. Please ensure your GPRN is in the subject line of the email."));
		}

		public async Task CannotSubmitMetersWhenLessThanActualCustomerCheckAsserts(string accountNumber,
																				   ClientAccountType accountType,
																				   List<MeterReadingData> meterReadings)
		{
			var cmd = new SubmitMeterReadingCommand(accountNumber, meterReadings, validateLastResults: true);
			App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(cmd, new DomainException(ResidentialDomainError.MeterReadingLessThanActualCustomer));
			var a = await Sut.ClickOnElement(Sut.SubmitButton);

			var errorPage = a.CurrentPageAs<SubmitMeterReadingsPage_SubmitErrorLessThanActual>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);

			Assert.IsTrue(errorPage.MeterReadingErrorTitle.TextContent.Contains("Sorry, we can’t take your reading right now"));
			Assert.IsTrue(errorPage.MeterReadingErrorMessage.TextContent.Contains("Sorry unfortunately we cannot accept your reading as it is less than the last customer read. If you are sure your reading is correct please send a dated photo of your meter to meterreadingchanges@electricireland.ie."));

			if (accountType == ClientAccountType.Electricity)
				Assert.IsTrue(errorPage.MeterReadingErrorMessage.TextContent.Contains("Please ensure your MPRN is in the subject line of the email."));

			if (accountType == ClientAccountType.Gas)
				Assert.IsTrue(errorPage.MeterReadingErrorMessage.TextContent.Contains("Please ensure your GPRN is in the subject line of the email."));
		}
	}
}