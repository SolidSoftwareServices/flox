using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.System;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.MovingHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.UI.TestServices.Flows.FlowInitializerUnitTest;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;
using ImpromptuInterface;
using Moq;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Steps
{
	[TestFixture]
	internal class FlowInitializerTests
	{
		private FlowInitializerTestConfigurator<FlowInitializer, ResidentialPortalFlowType> NewTestConfigurator(
			bool anonymousUser=false)
		{
			return NewTestConfigurator(anonymousUser
				? new DomainFacade()
				: new AppUserConfigurator().Execute().SetValidSession().DomainFacade);
		}

		private FlowInitializerTestConfigurator<FlowInitializer, ResidentialPortalFlowType> NewTestConfigurator(
			DomainFacade domainFacade)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowInitializerTestConfigurator<FlowInitializer, ResidentialPortalFlowType>(
					domainFacade.ModelsBuilder)
				.WithStub(domainFacade.SessionProvider)
				.WithStub(domainFacade.QueryResolver)
				.WithStub(domainFacade.CommandDispatcher);
		}

		[Test]
		[Theory]
		public async Task CanAuthorize(bool anonymousUser)
		{
			var expected = !anonymousUser;
			var actual = NewTestConfigurator(anonymousUser).Adapter.Authorize(false);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void FlowTypeIsCorrect()
		{
			Assert.AreEqual(ResidentialPortalFlowType.MovingHouse, NewTestConfigurator().Adapter.GetFlowType());
		}

		[Test,Theory]
		public async Task Can_InitializeContext_Accounts(bool mainAccountIsElectricityAccount, bool withDuelFuel)
		{

			var cfg = ConfigureDomain(mainAccountIsElectricityAccount, withDuelFuel);

			var mainAccountNumber = mainAccountIsElectricityAccount
				? cfg.ElectricityAccount().Model.AccountNumber
				: cfg.GasAccount().Model.AccountNumber;

			NewTestConfigurator(cfg.DomainFacade)
				.NewInitializationRunner()
				.WithInput<IMovingHouseInput>(new
				{
					InitiatedFromAccountNumber = mainAccountNumber
				}.ActLike<IMovingHouseInput>())
				.Run()
				.AssertTriggeredEventIs(actual => Assert.AreEqual(ScreenEvent.Start, actual))
				.AssertStepData<FlowInitializer.RootScreenModel>(actual =>
				{
					if (mainAccountIsElectricityAccount || withDuelFuel)
					{
						Assert.IsTrue(actual.ElectricityDevicesMeterReadings.First().MeterUnit == "kWh");
					}
					Assert.AreEqual(mainAccountNumber, actual.InitiatedFromAccountNumber);
					Assert.AreEqual(cfg.ElectricityAccount()?.Model.AccountNumber, actual.ElectricityAccountNumber);
					Assert.AreEqual(cfg.GasAccount()?.Model.AccountNumber, actual.GasAccountNumber);

				});
		}

		[Test, Theory]
		public async Task InitializeContext_Devices_Electricity(ConfigurableDeviceSet deviceSet)
		{
			if (deviceSet == ConfigurableDeviceSet.None||deviceSet==ConfigurableDeviceSet.Gas)
			{
				Assert.Ignore("Not applicable");
				return;
			}
			//lest configure the domain expectations of the scenario
			var cfg = new AppUserConfigurator()
				.SetValidSession();
			 cfg.AddElectricityAccount(configureDefaultDevice: false).WithDeviceSet(deviceSet);
			 cfg.Execute();

			 var account = cfg.ElectricityAccount();
			 //creates the test configurator telling it to use the domain configurations
			 NewTestConfigurator(cfg.DomainFacade)
				 //gets a test runner for testing the creation of the flow initialiser
				 .NewInitializationRunner()

				 //specifies the flow input for a given scenario
				 .WithInput<IMovingHouseInput>(new
				 {
					 InitiatedFromAccountNumber = account.Model.AccountNumber
				 }.ActLike<IMovingHouseInput>())
				 //Acts
				 .Run()
				 //assertions
				 .AssertTriggeredEventIs(actual => Assert.AreEqual(ScreenEvent.Start, actual))
				 .AssertStepData<FlowInitializer.RootScreenModel>(actual =>
				 {

					 Assert.AreEqual(
						 deviceSet == ConfigurableDeviceSet.Electricity24H ||
						 deviceSet == ConfigurableDeviceSet.ElectricityNightStorageHeater
							 ? 1
							 : 2, actual.ElectricityDevicesMeterReadings.Count());
					 Assert.AreEqual(account.Premise.ElectricityPrn.ToString(), actual.UserMeterInputFields.Mprn);
					 Assert.AreEqual(deviceSet == ConfigurableDeviceSet.Electricity24H || deviceSet == ConfigurableDeviceSet.Electricity24HAndNightStorageHeater,
						 actual.UserMeterInputFields.Electricity24HrsDevicesFieldRequired);
					 Assert.AreEqual(deviceSet == ConfigurableDeviceSet.Electricity24HAndNightStorageHeater || deviceSet == ConfigurableDeviceSet.ElectricityNightStorageHeater,
						 actual.UserMeterInputFields.ElectricityNightStorageHeaterDevicesFieldRequired);
					 Assert.AreEqual(deviceSet == ConfigurableDeviceSet.ElectricityDayAndNightMeters,
						 actual.UserMeterInputFields.ElectricityDayDevicesFieldRequired);
					 Assert.AreEqual(deviceSet == ConfigurableDeviceSet.ElectricityDayAndNightMeters,
						 actual.UserMeterInputFields.ElectricityNightDevicesFieldRequired);

				 });
		}
		[Test, Theory]
		public async Task InitializeContext_Devices_Gas()
		{
			//configure the domain
			var cfg = new AppUserConfigurator()
				.SetValidSession();
			cfg.AddGasAccount(configureDefaultDevice: false).WithDeviceSet(ConfigurableDeviceSet.Gas);
			cfg.Execute();

			var account = cfg.GasAccount();


			NewTestConfigurator(cfg.DomainFacade)
				.NewInitializationRunner()
				.WithInput(new
				{
					InitiatedFromAccountNumber = account.Model.AccountNumber
				}.ActLike<IMovingHouseInput>())
				.Run()
				.AssertTriggeredEventIs(actual => Assert.AreEqual(ScreenEvent.Start, actual))
				.AssertStepData<FlowInitializer.RootScreenModel>(actual =>
				{
					Assert.AreEqual(1, actual.GasDevicesMeterReadings.Count());
					Assert.AreEqual(account.Premise.GasPrn.ToString(), actual.UserMeterInputFields.Gprn);
					Assert.IsTrue(actual.UserMeterInputFields.GasDevicesFieldRequired);
				});
		}


		[Test]
		public async Task ItHandles_ErrorEvent()
		{
			NewTestConfigurator()
				.NewEventTestRunner()
				.WhenEvent(ScreenEvent.ErrorOccurred)
				.TheResultStepIs(MovingHouseStep.MovingHouseNotPresent);
		}

		[Test]
		public async Task ItHandles_StartEvent()
		{
			var rootStepData = new FlowInitializer.RootScreenModel();

			NewTestConfigurator()
				.NewEventTestRunner()
				.GivenTheStepDataIs(rootStepData)
				.WhenEvent(ScreenEvent.Start)
				.TheResultStepIs(MovingHouseStep.Step0OperationSelection);
		}


		private static AppUserConfigurator ConfigureDomain(bool mainAccountIsElectricityAccount, bool withDuelFuel)
		{
			var cfg = new AppUserConfigurator()
				.SetValidSession();
			var mainAccount = mainAccountIsElectricityAccount
				? (CommonElectricityAndGasAccountConfigurator)cfg.AddElectricityAccount()
				: cfg.AddGasAccount();
			if (withDuelFuel)
			{
				if (mainAccount.AccountType == ClientAccountType.Electricity)
					cfg.AddGasAccount(duelFuelSisterAccount: cfg.ElectricityAccount());
				else
					cfg.AddElectricityAccount(duelFuelSisterAccount: cfg.GasAccount());
			}

			cfg.Execute();
			return cfg;
		}

		private static IEnumerable<TestCaseData> ItHandlesEvents_WithMovingHouseValidationErrors()
		{
			var movingHouseValidationTypes = new List<MovingHouseValidationType>()
			{
				MovingHouseValidationType.IsNotPAYGCustomer,
				MovingHouseValidationType.IsNotLossInProgress,
				MovingHouseValidationType.IsResidentialCustomer,
				MovingHouseValidationType.IsSapCheckMoveOutOk,
				MovingHouseValidationType.BusinessAgreementDoesNotHaveCollectiveParent,
				MovingHouseValidationType.ContractEndDateIsValid,
				MovingHouseValidationType.DiscStatusIsNew,
				MovingHouseValidationType.HasPremiseInstallations,
			};

			foreach (var movingHouseValidationType in movingHouseValidationTypes)
			{
				//compose the test case
				yield return new TestCaseData(movingHouseValidationType);
			}
		}
		
		[TestCaseSource(nameof(ItHandlesEvents_WithMovingHouseValidationErrors))]
		public async Task It_GoesToMovingHouseValidationErrorScreen_WhenHasValidationError (MovingHouseValidationType movingHouseValidationType)
		{
			var rootStepData = new FlowInitializer.RootScreenModel
			{ 
				GasAccountNumber = "123",
				ElectricityAccountNumber = "345"
			};
			
			var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
			{
				Output = OutputType.Failed,
				MovingHouseValidationType = movingHouseValidationType
			};
			
			var domainFacade = new DomainFacade();
			domainFacade.QueryResolver.Current.Setup(
				x => 
				x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(
					It.IsAny<MovingHouseValidationQuery>(),
					 //new MovingHouseValidationQuery() {
					 //	GasAccountNumber = rootStepData.GasAccountNumber,
					 //	ElectricityAccountNumber = rootStepData.ElectricityAccountNumber
					 //}, //TODO ND: for some reason the IEquatable is not triggering
					 It.IsAny<bool>()))
				.Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));
			
			NewTestConfigurator(domainFacade)
				.NewEventTestRunner()
				.GivenTheStepDataIs(rootStepData)
				.WhenEvent(ScreenEvent.Start)
				.TheResultStepIs(MovingHouseStep.ShowMovingHouseValidationError);
		}
	}
}