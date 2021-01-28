using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Metering.Premises;
using EI.RP.DomainServices.Queries.MovingHouse.Progress;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;


namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Steps
{
	internal class RequiredIfTestCaseData
	{
		public AccountsForMovingTypeBuilder AccountsForMovingTypeBuilder { get; set; }

		public bool Electricity24HrsDevicesFieldRequired { get; set; }
		public bool ElectricityDayDevicesFieldRequired { get; set; }
		public bool ElectricityNightDevicesFieldRequired { get; set; }
		public bool ElectricityNightStorageHeaterDevicesFieldRequired { get; set; }
	}

	[TestFixture]
	internal class Step3InputMoveInPropertyDetailsTests
    {
	    Premise GetPremiseInfo(bool electricityNightStorageHeaterDevicesFieldRequired , bool electricity24HrsDevicesFieldRequired, bool electricityNightDevicesFieldRequired, bool electricityDayDevicesFieldRequired)
	    {
		    var registers =  new List<DeviceRegisterInfo>();
		    if (electricityNightStorageHeaterDevicesFieldRequired)
		    {
			    registers.Add(new DeviceRegisterInfo
			    {
				    MeterType = MeterType.ElectricityNightStorageHeater
			    });
		    }

		    if (electricity24HrsDevicesFieldRequired)
		    {
			    registers.Add(new DeviceRegisterInfo
			    {
				    MeterType = MeterType.Electricity24h
			    });
		    }

		    if (electricityNightDevicesFieldRequired)
		    {
			    registers.Add(new DeviceRegisterInfo
			    {
				    MeterType = MeterType.ElectricityNight
			    });
		    }

		    if (electricityDayDevicesFieldRequired)
		    {
			    registers.Add(new DeviceRegisterInfo
			    {
				    MeterType = MeterType.ElectricityDay
			    });
		    }

			return new Premise
		    {
			    Installations = new List<InstallationInfo>
			    {
				    new InstallationInfo
				    {
					    Devices = new List<DeviceInfo>
					    {
						    new DeviceInfo
						    {
							    MeterReadingResults = new List<MeterReadingInfo>()
							    {
								    new MeterReadingInfo()
							    },

								Registers = registers
							},
					    }
				    }
			    }
		    };
	    }

		private FlowScreenTestConfigurator<Step3InputMoveInPropertyDetails, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade = null)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowScreenTestConfigurator<Step3InputMoveInPropertyDetails, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
				//Assigns the domain stubs to the configurator to be injected in the step instances(see other methods)
				.WithStub(domainFacade.SessionProvider)
				.WithStub(domainFacade.QueryResolver)
				.WithStub(domainFacade.CommandDispatcher);
		}

        private static IEnumerable<TestCaseData> MovingHouseTestCases(string caseName)
        {
            var movingHouseTypes = MovingHouseType.AllValues.Where(x =>
                    !x.IsOneOf(MovingHouseType.CloseElectricityAndGas, MovingHouseType.CloseElectricity,
                        MovingHouseType.CloseGas))
                .Cast<MovingHouseType>();

            foreach (var movingType in movingHouseTypes)
            {
                //compose the test case
                yield return new TestCaseData(new AccountsForMovingTypeBuilder(movingType) )
                    .SetName($"{caseName}_{movingType}");
            }
        }

        private static IEnumerable<TestCaseData> MovingHouseTestCasesElectricityOnly(string caseName)
        {
	        var movingHouseTypes = MovingHouseType.AllValues.Where(x =>
			        !x.IsOneOf(MovingHouseType.CloseElectricityAndGas, 
				        MovingHouseType.CloseElectricity,
				        MovingHouseType.CloseGas,
				        MovingHouseType.MoveGas,
				        MovingHouseType.MoveGasAndAddElectricity))
		        .Cast<MovingHouseType>();

	        foreach (var movingType in movingHouseTypes)
	        {
		        yield return new TestCaseData(new RequiredIfTestCaseData { ElectricityNightStorageHeaterDevicesFieldRequired = false, Electricity24HrsDevicesFieldRequired = false, ElectricityNightDevicesFieldRequired = false, ElectricityDayDevicesFieldRequired = false, AccountsForMovingTypeBuilder = new AccountsForMovingTypeBuilder(movingType) }).SetName($"{caseName}_{movingType}_FFFF");
		        yield return new TestCaseData(new RequiredIfTestCaseData { ElectricityNightStorageHeaterDevicesFieldRequired = false, Electricity24HrsDevicesFieldRequired = false, ElectricityNightDevicesFieldRequired = false, ElectricityDayDevicesFieldRequired = true,  AccountsForMovingTypeBuilder = new AccountsForMovingTypeBuilder(movingType) }).SetName($"{caseName}_{movingType}_FFFT");
		        yield return new TestCaseData(new RequiredIfTestCaseData { ElectricityNightStorageHeaterDevicesFieldRequired = false, Electricity24HrsDevicesFieldRequired = false, ElectricityNightDevicesFieldRequired = true,  ElectricityDayDevicesFieldRequired = true,  AccountsForMovingTypeBuilder = new AccountsForMovingTypeBuilder(movingType) }).SetName($"{caseName}_{movingType}_FFTT");
		        yield return new TestCaseData(new RequiredIfTestCaseData { ElectricityNightStorageHeaterDevicesFieldRequired = false, Electricity24HrsDevicesFieldRequired = true,  ElectricityNightDevicesFieldRequired = true,  ElectricityDayDevicesFieldRequired = true,  AccountsForMovingTypeBuilder = new AccountsForMovingTypeBuilder(movingType) }).SetName($"{caseName}_{movingType}_FTTT");
		        yield return new TestCaseData(new RequiredIfTestCaseData { ElectricityNightStorageHeaterDevicesFieldRequired = true,  Electricity24HrsDevicesFieldRequired = true,  ElectricityNightDevicesFieldRequired = true,  ElectricityDayDevicesFieldRequired = true,  AccountsForMovingTypeBuilder = new AccountsForMovingTypeBuilder(movingType) }).SetName($"{caseName}_{movingType}_TTTT");
		        yield return new TestCaseData(new RequiredIfTestCaseData { ElectricityNightStorageHeaterDevicesFieldRequired = true,  Electricity24HrsDevicesFieldRequired = true,  ElectricityNightDevicesFieldRequired = true,  ElectricityDayDevicesFieldRequired = false, AccountsForMovingTypeBuilder = new AccountsForMovingTypeBuilder(movingType) }).SetName($"{caseName}_{movingType}_TTTF");
		        yield return new TestCaseData(new RequiredIfTestCaseData { ElectricityNightStorageHeaterDevicesFieldRequired = true,  Electricity24HrsDevicesFieldRequired = true,  ElectricityNightDevicesFieldRequired = false, ElectricityDayDevicesFieldRequired = false, AccountsForMovingTypeBuilder = new AccountsForMovingTypeBuilder(movingType) }).SetName($"{caseName}_{movingType}_TTFF");
		        yield return new TestCaseData(new RequiredIfTestCaseData { ElectricityNightStorageHeaterDevicesFieldRequired = true,  Electricity24HrsDevicesFieldRequired = false, ElectricityNightDevicesFieldRequired = false, ElectricityDayDevicesFieldRequired = false, AccountsForMovingTypeBuilder = new AccountsForMovingTypeBuilder(movingType) }).SetName($"{caseName}_{movingType}_TFFF");
		        yield return new TestCaseData(new RequiredIfTestCaseData { ElectricityNightStorageHeaterDevicesFieldRequired = true,  Electricity24HrsDevicesFieldRequired = false, ElectricityNightDevicesFieldRequired = true,  ElectricityDayDevicesFieldRequired = false, AccountsForMovingTypeBuilder = new AccountsForMovingTypeBuilder(movingType) }).SetName($"{caseName}_{movingType}_TFTF");
		        yield return new TestCaseData(new RequiredIfTestCaseData { ElectricityNightStorageHeaterDevicesFieldRequired = false, Electricity24HrsDevicesFieldRequired = true,  ElectricityNightDevicesFieldRequired = false, ElectricityDayDevicesFieldRequired = true,  AccountsForMovingTypeBuilder = new AccountsForMovingTypeBuilder(movingType) }).SetName($"{caseName}_{movingType}_FTFT");
	        }
        }
		
		[TestCaseSource(nameof(MovingHouseTestCasesElectricityOnly), new object[] { nameof(ItSetsTheCorrectRequiredIfValuesOnScreenModel) })]
		public void ItSetsTheCorrectRequiredIfValuesOnScreenModel(RequiredIfTestCaseData requiredIfTestCaseData)
		{
			var domainConfiguration = requiredIfTestCaseData.AccountsForMovingTypeBuilder.Create();

			//create builders that help creating reusable scenario parts
			var rootDataBuilder = new RootDataBuilder(domainConfiguration, requiredIfTestCaseData.AccountsForMovingTypeBuilder.MovingType);

			var step0DataBuilder = new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
			var step1DataBuilder = new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder);
			var step2DataBuilder = new Step2InputPrnsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder, step1DataBuilder);
			var step3DataBuilder = new Step3InputMoveInPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder);

			//build the state datas
			var rootStepData = rootDataBuilder.Create();
			var step0Data = step0DataBuilder.Create();
			var step1Data = step1DataBuilder.Create();
			var step2Data = step2DataBuilder.Create();

			step2Data.PremiseAddressInfos.First().PremiseName = "MPRN";
			var step3Data = step3DataBuilder.Create();

			domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
				x => x.FetchAsync<PremisesQuery, Premise>(It.IsAny<PremisesQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(GetPremiseInfo(
					requiredIfTestCaseData.ElectricityNightStorageHeaterDevicesFieldRequired,
					requiredIfTestCaseData.Electricity24HrsDevicesFieldRequired,
					requiredIfTestCaseData.ElectricityNightDevicesFieldRequired,
					requiredIfTestCaseData.ElectricityDayDevicesFieldRequired).ToOneItemArray().AsEnumerable()));

			domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
					x => x.FetchAsync<MoveHouseProgressQuery, MovingHouseInProgressMovingInInfo>(
						It.IsAny<MoveHouseProgressQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(GetMovingHouseInProgressMovingInInfo().ToOneItemArray().AsEnumerable()));

			//build the unit test configurator
			NewScreenTestConfigurator(domainConfiguration.DomainFacade)
				.NewTestCreateStepDataRunner()
				//arrange the scenario by setting the user selections in the previous steps
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)
				.WithExistingStepData(MovingHouseStep.Step1InputMoveOutPropertyDetails, step1Data)
				.WithExistingStepData(MovingHouseStep.Step2InputPrns, step2Data)

				.WhenCreated()
				.ThenTheStepDataIs<Step3InputMoveInPropertyDetails.ScreenModel>((stepData) =>
				{
					Assert.AreEqual(stepData.UserMeterInputFields.ElectricityNightStorageHeaterDevicesFieldRequired, requiredIfTestCaseData.ElectricityNightStorageHeaterDevicesFieldRequired);
					Assert.AreEqual(stepData.UserMeterInputFields.Electricity24HrsDevicesFieldRequired, requiredIfTestCaseData.Electricity24HrsDevicesFieldRequired);
					Assert.AreEqual(stepData.UserMeterInputFields.ElectricityNightDevicesFieldRequired, requiredIfTestCaseData.ElectricityNightDevicesFieldRequired);
					Assert.AreEqual(stepData.UserMeterInputFields.ElectricityDayDevicesFieldRequired, requiredIfTestCaseData.ElectricityDayDevicesFieldRequired);
				});

			MovingHouseInProgressMovingInInfo GetMovingHouseInProgressMovingInInfo()
			{
				return new MovingHouseInProgressMovingInInfo();
			}
		}

		[TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_ToStep4OnMoveInDateMoreThan2daysInTheFuturePassed) })]
        public void ItHandlesEvent_To_ToStep4OnMoveInDateMoreThan2daysInTheFuturePassed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
        {
            var domainConfiguration = accountsForMovingTypeBuilder.Create();

            //create builders that help creating reusable scenario parts
            var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);

            var step0DataBuilder = new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
            var step1DataBuilder = new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder);
            var step2DataBuilder = new Step2InputPrnsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder, step1DataBuilder);
            var step3DataBuilder = new Step3InputMoveInPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder);

            //build the state datas
            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0DataBuilder.Create();
            var step1Data = step1DataBuilder.Create();
            var step2Data = step2DataBuilder.Create();
            var step3Data = step3DataBuilder.Create();

            var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Passed,
                MovingHouseValidationType = MovingHouseValidationType.MoveInDateMoreThan2daysInTheFuture
            };

            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step3Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)
                .WithExistingStepData(MovingHouseStep.Step1InputMoveOutPropertyDetails, step1Data)
                .WithExistingStepData(MovingHouseStep.Step2InputPrns, step2Data)

            //act
            .WhenEvent(Step3InputMoveInPropertyDetails.StepEvent.ToStep4)

            //assert UI
            .ThenTheResultStepIs(MovingHouseStep.Step4ConfigurePayment)
            .ThenTheValidationPassed();
        }

		[TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandles_ErrorEvent) })]
		public async Task ItHandles_ErrorEvent(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
		{
			var domainConfiguration = accountsForMovingTypeBuilder.Create();
	        var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);

			var step0DataBuilder = new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
			var step1DataBuilder = new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder);
			var step2DataBuilder = new Step2InputPrnsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder, step1DataBuilder);
			var step3DataBuilder = new Step3InputMoveInPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder);

			var rootStepData = rootDataBuilder.Create();
			var step0Data = step0DataBuilder.Create();
			var step1Data = step1DataBuilder.Create();
			var step2Data = step2DataBuilder.Create();
			var step3Data = step3DataBuilder.Create();

			NewScreenTestConfigurator(domainConfiguration.DomainFacade)
		        //get a runner to test events
		        .NewEventTestRunner(step3Data)
		        //arrange the scenario by setting the user selections in the previous steps
		        .WithExistingStepData(ScreenName.PreStart, rootStepData)
		        .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)
		        .WithExistingStepData(MovingHouseStep.Step1InputMoveOutPropertyDetails, step1Data)
		        .WithExistingStepData(MovingHouseStep.Step2InputPrns, step2Data)

				.WhenEvent(ScreenEvent.ErrorOccurred)
		        //assert UI
		        .ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseUnhandledError);
        }

		[TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_MovingHouseValidationOnMoveInDateMoreThan2daysInTheFutureFailed) })]
        public void ItHandlesEvent_To_MovingHouseValidationOnMoveInDateMoreThan2daysInTheFutureFailed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
        {
            var domainConfiguration = accountsForMovingTypeBuilder.Create();

            //create builders that help creating reusable scenario parts
            var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);

            var step0DataBuilder = new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
            var step1DataBuilder = new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder);
            var step2DataBuilder = new Step2InputPrnsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder, step1DataBuilder);
            var step3DataBuilder = new Step3InputMoveInPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder);

            //build the state datas
            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0DataBuilder.Create();
            var step1Data = step1DataBuilder.Create();
            var step2Data = step2DataBuilder.Create();
            var step3Data = step3DataBuilder.Create();
            step3Data.MoveInOutDatePicker.MovingInOutSelectedDateTime = DateTime.Today.AddDays(3);

            var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Failed,
                MovingHouseValidationType = MovingHouseValidationType.MoveInDateMoreThan2daysInTheFuture
            };

            //domainConfigurator.DomainFacade.QueryResolver.ExpectQuery. TODO : does not seem to be triggering IEquatable logic in query
            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step3Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)
                .WithExistingStepData(MovingHouseStep.Step1InputMoveOutPropertyDetails, step1Data)
                .WithExistingStepData(MovingHouseStep.Step2InputPrns, step2Data)

            //act
            .WhenEvent(Step3InputMoveInPropertyDetails.StepEvent.ToStep4)

            //assert UI
            .ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseValidationError)
            .ThenTheValidationPassed();
        }

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_MovingHouseHasElectricNewPremiseAccountDetailsInSwitchFailed) })]
        public void ItHandlesEvent_To_MovingHouseHasElectricNewPremiseAccountDetailsInSwitchFailed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
        {
            var domainConfiguration = accountsForMovingTypeBuilder.Create();

            //create builders that help creating reusable scenario parts
            var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);

            var step0DataBuilder = new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
            var step1DataBuilder = new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder);
            var step2DataBuilder = new Step2InputPrnsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder, step1DataBuilder);
            var step3DataBuilder = new Step3InputMoveInPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder);

            //build the state datas
            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0DataBuilder.Create();
            var step1Data = step1DataBuilder.Create();
            var step2Data = step2DataBuilder.Create();
            var step3Data = step3DataBuilder.Create();

            var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Failed,
                MovingHouseValidationType = MovingHouseValidationType.HasElectricNewPremiseAccountDetailsInSwitch
            };

            //domainConfigurator.DomainFacade.QueryResolver.ExpectQuery. TODO : does not seem to be triggering IEquatable logic in query
            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step3Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)
                .WithExistingStepData(MovingHouseStep.Step1InputMoveOutPropertyDetails, step1Data)
                .WithExistingStepData(MovingHouseStep.Step2InputPrns, step2Data)

            //act
            .WhenEvent(Step3InputMoveInPropertyDetails.StepEvent.ToStep4)

            //assert UI
            .ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseValidationError)
            .ThenTheValidationPassed();
        }

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_ToStep4OnHasElectricNewPremiseAccountDetailsInSwitchPassed) })]
        public void ItHandlesEvent_To_ToStep4OnHasElectricNewPremiseAccountDetailsInSwitchPassed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
        {
            var domainConfiguration = accountsForMovingTypeBuilder.Create();

            //create builders that help creating reusable scenario parts
            var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);

            var step0DataBuilder = new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
            var step1DataBuilder = new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder);
            var step2DataBuilder = new Step2InputPrnsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder, step1DataBuilder);
            var step3DataBuilder = new Step3InputMoveInPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder);

            //build the state datas
            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0DataBuilder.Create();
            var step1Data = step1DataBuilder.Create();
            var step2Data = step2DataBuilder.Create();
            var step3Data = step3DataBuilder.Create();

            var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Passed,
                MovingHouseValidationType = MovingHouseValidationType.HasElectricNewPremiseAccountDetailsInSwitch
            };

            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step3Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)
                .WithExistingStepData(MovingHouseStep.Step1InputMoveOutPropertyDetails, step1Data)
                .WithExistingStepData(MovingHouseStep.Step2InputPrns, step2Data)

            //act
            .WhenEvent(Step3InputMoveInPropertyDetails.StepEvent.ToStep4)

            //assert UI
            .ThenTheResultStepIs(MovingHouseStep.Step4ConfigurePayment)
            .ThenTheValidationPassed();
        }

        [Test]
        public void FlowIsCorrect()
        {
            Assert.AreEqual(ResidentialPortalFlowType.MovingHouse, NewScreenTestConfigurator().Adapter.GetFlowType());
        }

        [Test]
        public void ScreenStepIsCorrect()
        {
            Assert.AreEqual(MovingHouseStep.Step3InputMoveInPropertyDetails,
                NewScreenTestConfigurator().Adapter.GetStep());
        }

        [Test]
        public void ViewPathIsCorrect()
        {
            Assert.AreEqual("Step3InputMoveInPropertyDetails", NewScreenTestConfigurator().Adapter.GetViewPath());
        }
    }
}