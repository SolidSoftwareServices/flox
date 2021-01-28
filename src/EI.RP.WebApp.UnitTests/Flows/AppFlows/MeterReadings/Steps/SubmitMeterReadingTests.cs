using System.Linq;
using System.Collections.Generic;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.Steps;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.MeterReadings.Infrastructure.StepsDataBuilder;
using EI.RP.DomainServices.Commands.Metering.SubmitMeterReading;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainErrors;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MeterReadings.Steps
{
	[TestFixture]
	internal class SubmitMeterReadingTests
	{
		private FlowScreenTestConfigurator<SubmitMeterReading, ResidentialPortalFlowType>
			NewScreenTestConfigurator(DomainFacade domainFacade = null)
		{
			//creates the domain layer facade
			if (domainFacade == null) domainFacade = new DomainFacade();
			//creates a flo test configurator
			return new FlowScreenTestConfigurator<SubmitMeterReading, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
				//Assigns the domain stubs to the configurator to be injected in the step instances(see other methods)
				.WithStub(domainFacade.SessionProvider)
				.WithStub(domainFacade.QueryResolver)
				.WithStub(domainFacade.CommandDispatcher);
		}

		public static IEnumerable<TestCaseData> SubmitMeterReadings_Cases(string caseName)
		{
			//single meter reading
			var cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricity24HrsDevices();
			yield return new TestCaseData(cfg).SetName($"{caseName} 24H device, 1 input field");

			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
			.WithElectricityNightStorageHeaterDevice();
			yield return new TestCaseData(cfg).SetName($"{caseName} NSH device, 1 input field");

			// MCC02, 2 meter readings with diffrerent meter numers in same device
			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricityDayAndNightDevices();
			yield return new TestCaseData(cfg).SetName($"{caseName} MCC02, D/N, 2 input fields");

			//2 meter readings different devices
			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricity24HrsDevices()
				.WithElectricityNightStorageHeaterDevice();
			yield return new TestCaseData(cfg).SetName($"{caseName} 24 + NSH, 2 input fields");

			//2 meter readings with same D/N meters numers in 1st device and NSH in 2nd device
			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricityDayAndNightDevices()
				.WithElectricityNightStorageHeaterDevice();
			yield return new TestCaseData(cfg).SetName($"{caseName} MCC51 D/N+NSH, fields 3");

			// MCC53, 2x D/N in different devices, different meter numbers
			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricityDayAndNightDevices()
				.WithElectricityDayAndNightDevices();
			yield return new TestCaseData(cfg).SetName($"{caseName} MCC53 2 x D/N, input fields 4");

			// MCC58 24h+D/N, 3 input fields
			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricity24HrsDevices()
				.WithElectricityDayAndNightDevices();
			yield return new TestCaseData(cfg).SetName($"{caseName} MCC58 24h+D/N, input fields 3");

			// MCC60 24h+D/N+NS, 4 input fields
			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricity24HrsDevices()
				.WithElectricityDayAndNightDevices()
				.WithElectricityNightStorageHeaterDevice();
			yield return new TestCaseData(cfg).SetName($"{caseName} MCC60 24h+D/N+NS, input fields 4");

			// MCC61 2x24h, 2 input fields
			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricity24HrsDevices()
				.WithElectricity24HrsDevices();
			yield return new TestCaseData(cfg).SetName($"{caseName} MCC61 2x24h, input fields 2");

			// MCC62 2x24h+NSH, 3 input fields
			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricity24HrsDevices()
				.WithElectricity24HrsDevices()
				.WithElectricityNightStorageHeaterDevice();
			yield return new TestCaseData(cfg).SetName($"{caseName} MCC62 2x24h+NSH, input fields 3");

			//MCC65 3X24h, 3 input fields
			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricity24HrsDevices()
				.WithElectricity24HrsDevices()
				.WithElectricity24HrsDevices();
			yield return new TestCaseData(cfg).SetName($"{caseName} MCC65 3X24, input fields 3");

			//MCC73 4X24+NSH, 5 input fields
			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricity24HrsDevices()
				.WithElectricity24HrsDevices()
				.WithElectricity24HrsDevices()
				.WithElectricity24HrsDevices()
				.WithElectricityNightStorageHeaterDevice();
			yield return new TestCaseData(cfg).SetName($"{caseName} MCC73 4X24+NSH, input fields 5");

			//MCC75, 3 meter readings different devices
			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricity24HrsDevices()
				.WithElectricityNightStorageHeaterDevice()
				.WithElectricityNightStorageHeaterDevice();
			yield return new TestCaseData(cfg).SetName($"{caseName} MCC75, 24+2XNSH, input fields 3");

			//MCC79 5X24+NSH, 6 input fields
			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricity24HrsDevices()
				.WithElectricity24HrsDevices()
				.WithElectricity24HrsDevices()
				.WithElectricity24HrsDevices()
				.WithElectricity24HrsDevices()
				.WithElectricityNightStorageHeaterDevice();
			yield return new TestCaseData(cfg).SetName($"{caseName} MCC79, 5X24+NSH, input fields 6");

			//gas meter reading
			cfg = new AppUserConfigurator(new DomainFacade());
			cfg.AddGasAccount(configureDefaultDevice: false)
				.WithGasDevice();
			yield return new TestCaseData(cfg).SetName($"{caseName} gas device, 1 input field");
		}

		[TestCaseSource(nameof(SubmitMeterReadings_Cases), new object[] { nameof(ItCanSubmitMeterReadings) })]
		public void ItCanSubmitMeterReadings(AppUserConfigurator configuratorWithDevices)
		{
			//Arrange
			configuratorWithDevices.Execute();

			var rootDataBuilder = new RootDataBuilder(configuratorWithDevices);
			var stepDataBuilder = new SubmitMeterReadingStepDataBuilder(configuratorWithDevices);
			var historyMeterReadsDataBuilder = new MeterReadingHistoryDataBuilder();

			var stepData = stepDataBuilder.Create();
			var account = configuratorWithDevices.ElectricityAndGasAccountConfigurators.Single();

			var meterReadingDataResults = new List<MeterReadingData>();
			foreach (var meterReading in stepData.MeterReadings)
			{
				var meterData = new MeterReadingData
				{
					DeviceId = meterReading.DeviceId,
					MeterNumber = meterReading.MeterNumber,
					MeterReading = meterReading.ReadingValue,
					RegisterId = meterReading.RegisterId,
					MeterTypeName = meterReading.MeterTypeName
				};

				meterReadingDataResults.Add(meterData);
			}

			var command = new SubmitMeterReadingCommand(account.Model.AccountNumber, meterReadingDataResults, validateLastResults: true);
			configuratorWithDevices.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(command);

			NewScreenTestConfigurator(configuratorWithDevices.DomainFacade)
				.NewEventTestRunner(stepData)
				.WithExistingStepData(ScreenName.PreStart, rootDataBuilder.Create())
				//act
				.WhenEvent(SubmitMeterReading.StepEvent.SubmitMeterReading)

				//flow step assertions 
				.ThenTheValidationPassed()
				.ThenTheResultStepIs(MeterReadingStep.ShowMeterReadingStatus)
				.ThenTheStepDataAfterIs<SubmitMeterReading.ScreenModel>(actual =>
					{
						Assert.AreEqual(account.Model.AccountNumber, actual.AccountNumber);
						Assert.IsFalse(actual.HasMeterReadingLessThanActualNetworkError);
						Assert.IsFalse(actual.HasMeterReadingLessThanActualCustomerError);
					}
				);

			//domain assertions(interaction evidences with the domain facade)
			configuratorWithDevices.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(command);
		}

		[TestCaseSource(nameof(SubmitMeterReadings_Cases), new object[] { nameof(ItCannotSubmitMeterReadingsLessThanNetworkActual) })]
		public void ItCannotSubmitMeterReadingsLessThanNetworkActual(AppUserConfigurator configuratorWithDevices)
		{
			//Arrange
			configuratorWithDevices.Execute();

			var rootDataBuilder = new RootDataBuilder(configuratorWithDevices);
			var stepDataBuilder = new SubmitMeterReadingStepDataBuilder(configuratorWithDevices);
			var historyMeterReadsDataBuilder = new MeterReadingHistoryDataBuilder();

			var stepData = stepDataBuilder.Create();
			var account = configuratorWithDevices.ElectricityAndGasAccountConfigurators.Single();

			var meterReadingDataResults = new List<MeterReadingData>();

			foreach (var meterReading in stepData.MeterReadings)
			{
				var meterData = new MeterReadingData
				{
					DeviceId = meterReading.DeviceId,
					MeterNumber = meterReading.MeterNumber,
					MeterReading = meterReading.ReadingValue,
					RegisterId = meterReading.RegisterId,
					MeterTypeName = meterReading.MeterTypeName
				};

				meterReadingDataResults.Add(meterData);
			}

			var command = new SubmitMeterReadingCommand(account.Model.AccountNumber, meterReadingDataResults, validateLastResults: true);
			configuratorWithDevices.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(command, new DomainException(ResidentialDomainError.MeterReadingLessThanActualNetwork));

			NewScreenTestConfigurator(configuratorWithDevices.DomainFacade)
				.NewEventTestRunner(stepData)
				.WithExistingStepData(ScreenName.PreStart, rootDataBuilder.Create())
				//act
				.WhenEvent(SubmitMeterReading.StepEvent.SubmitMeterReading)

				//flow step assertions 
				.ThenTheValidationPassed()
				.ThenTheResultStepIs(MeterReadingStep.ErrorSubmittedLessThanActual)
				.ThenTheStepDataAfterIs<SubmitMeterReading.ScreenModel>(actual =>
					{
						Assert.AreEqual(account.Model.AccountNumber, actual.AccountNumber);
						Assert.IsTrue(actual.HasMeterReadingLessThanActualNetworkError);
						Assert.IsFalse(actual.HasMeterReadingLessThanActualCustomerError);
					}
				);

			//domain assertions(interaction evidences with the domain facade)
			configuratorWithDevices.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(command);
		}

		[TestCaseSource(nameof(SubmitMeterReadings_Cases), new object[] { nameof(ItCannotSubmitMeterReadingsLessThanCustomerActual) })]
		public void ItCannotSubmitMeterReadingsLessThanCustomerActual(AppUserConfigurator configuratorWithDevices)
		{
			//Arrange
			configuratorWithDevices.Execute();

			var rootDataBuilder = new RootDataBuilder(configuratorWithDevices);
			var stepDataBuilder = new SubmitMeterReadingStepDataBuilder(configuratorWithDevices);
			var historyMeterReadsDataBuilder = new MeterReadingHistoryDataBuilder();

			var stepData = stepDataBuilder.Create();
			var account = configuratorWithDevices.ElectricityAndGasAccountConfigurators.Single();

			var meterReadingDataResults = new List<MeterReadingData>();
			foreach (var meterReading in stepData.MeterReadings)
			{
				var meterData = new MeterReadingData
				{
					DeviceId = meterReading.DeviceId,
					MeterNumber = meterReading.MeterNumber,
					MeterReading = meterReading.ReadingValue,
					RegisterId = meterReading.RegisterId,
					MeterTypeName = meterReading.MeterTypeName
				};

				meterReadingDataResults.Add(meterData);
			}

			var command = new SubmitMeterReadingCommand(account.Model.AccountNumber, meterReadingDataResults, validateLastResults: true);
			configuratorWithDevices.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(command, new DomainException(ResidentialDomainError.MeterReadingLessThanActualCustomer));

			NewScreenTestConfigurator(configuratorWithDevices.DomainFacade)
				.NewEventTestRunner(stepData)
				.WithExistingStepData(ScreenName.PreStart, rootDataBuilder.Create())
				//act
				.WhenEvent(SubmitMeterReading.StepEvent.SubmitMeterReading)

				//flow step assertions 
				.ThenTheValidationPassed()
				.ThenTheResultStepIs(MeterReadingStep.ErrorSubmittedLessThanActual)
				.ThenTheStepDataAfterIs<SubmitMeterReading.ScreenModel>(actual =>
					{
						Assert.AreEqual(account.Model.AccountNumber, actual.AccountNumber);
						Assert.IsTrue(actual.HasMeterReadingLessThanActualCustomerError);
						Assert.IsFalse(actual.HasMeterReadingLessThanActualNetworkError);
					}
				);

			//domain assertions(interaction evidences with the domain facade)
			configuratorWithDevices.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(command);
		}
	}
}