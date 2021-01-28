using System;
using AutoFixture;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;
using NUnit.Framework;
using Moq.AutoMock;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;

namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.Validation.RuleValidators
{
    [TestFixture]
    internal class PodNotRegisteredEarlierTodayValidatorUnitTests : UnitTestFixture<
        PodNotRegisteredEarlierTodayValidatorUnitTests.TestContext,
        PodNotRegisteredEarlierTodayValidator>
    {
        internal class CaseModel
        {
            public string CaseName { get; set; }
            public bool ValidateNewPremisePodNotRegisteredToday { get; set; }
            public bool ElectricAccount { get; set; }
            public bool ElectricAccountHasDevices { get; set; }
            public bool IsMPRNDeregistered { get; set; }
            public bool IsGPRNDeregistered { get; set; }
			public bool IsPodRegisteredToday { get; set; }
            public bool GasAccount { get; set; }
            public bool GasAccountHasDevices { get; set; }
            public OutputType Output { get; set; }
        }

        public static IEnumerable<TestCaseData> CanResolveCases()
        {
            var cases = new CaseModel[]
            {
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = true, IsMPRNDeregistered = false, GasAccount = false, GasAccountHasDevices = false, IsGPRNDeregistered = false, IsPodRegisteredToday=false, CaseName = "PodNotRegToday-HasElecDevicesAndConnection", Output = OutputType.Passed },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = true, IsMPRNDeregistered = false, GasAccount = false, GasAccountHasDevices = false, IsGPRNDeregistered = false, IsPodRegisteredToday=true, CaseName = "PodRegToday-HasElecDevicesAndConnection", Output = OutputType.Failed },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = false, IsMPRNDeregistered = false, GasAccount = false, GasAccountHasDevices = false, IsGPRNDeregistered = false, IsPodRegisteredToday=true, CaseName = "PodNotRegToday-HasElecConnectionButNoDevices", Output = OutputType.NotExecuted},
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = false, ElectricAccount = true, ElectricAccountHasDevices = false, IsMPRNDeregistered = false, GasAccount = false, GasAccountHasDevices = false, IsGPRNDeregistered = false, CaseName = "DoNotValidate", Output = OutputType.NotExecuted },

                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = false, ElectricAccountHasDevices = false, IsMPRNDeregistered = false, GasAccount = true, GasAccountHasDevices = true, IsGPRNDeregistered = false, IsPodRegisteredToday= false, CaseName = "PodNotRegToday-HasGasConnectionAndDevices", Output = OutputType.Passed },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = false, ElectricAccountHasDevices = false, IsMPRNDeregistered = false, GasAccount = true, GasAccountHasDevices = true, IsGPRNDeregistered = false, IsPodRegisteredToday= true, CaseName = "PodRegToday-HasGasConnectionButNoDevices", Output = OutputType.Failed },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = false, ElectricAccountHasDevices = false, IsMPRNDeregistered = false, GasAccount = true, GasAccountHasDevices = false, IsGPRNDeregistered = false, IsPodRegisteredToday= false, CaseName = "PodNotRegToday-HasGasConnectionButNoDevices", Output = OutputType.NotExecuted },

                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = true, IsMPRNDeregistered = false, GasAccount = true, GasAccountHasDevices = true, IsGPRNDeregistered = false, IsPodRegisteredToday=false, CaseName = "PodNotRegToday-HasBothDevicesAndConnection",Output = OutputType.Passed },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = false, IsMPRNDeregistered = false, GasAccount = true, GasAccountHasDevices = true, IsGPRNDeregistered = false, IsPodRegisteredToday=false, CaseName = "PodNotRegToday-HasBothConnectionWithGasDevices" ,Output = OutputType.Passed },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = true, IsMPRNDeregistered = false, GasAccount = true, GasAccountHasDevices = false, IsGPRNDeregistered = false, IsPodRegisteredToday=false, CaseName = "PodNotRegToday-HasBothConnectionWithElecDevices" ,Output = OutputType.Passed },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = true, IsMPRNDeregistered = false, GasAccount = true, GasAccountHasDevices = true, IsGPRNDeregistered = false, IsPodRegisteredToday=true, CaseName = "PodRegToday-HasBothDevicesAndConnection" ,Output = OutputType.Failed },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = false, IsMPRNDeregistered = false, GasAccount = true, GasAccountHasDevices = true, IsGPRNDeregistered = false, IsPodRegisteredToday=true, CaseName = "PodRegToday-HasBothConnectionWithGasDevices" ,Output = OutputType.Failed },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = true, IsMPRNDeregistered = false, GasAccount = true, GasAccountHasDevices = false, IsGPRNDeregistered = false, IsPodRegisteredToday=true, CaseName = "PodRegToday-HasBothConnectionWithElecDevices" ,Output = OutputType.Failed },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = false, IsMPRNDeregistered = false, GasAccount = true, GasAccountHasDevices = false, IsGPRNDeregistered = false, IsPodRegisteredToday=false, CaseName = "PodNotRegToday-HasBothConnectionWithNoDevices" ,Output = OutputType.NotExecuted },

                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = false, IsMPRNDeregistered = true, GasAccount = false, GasAccountHasDevices = false, IsGPRNDeregistered = false, IsPodRegisteredToday = false, CaseName = "PodRegToday-MPRN_Dereg_NoDevices_NoGas", Output = OutputType.NotExecuted },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = true, IsMPRNDeregistered = true, GasAccount = false, GasAccountHasDevices = false, IsGPRNDeregistered = false, IsPodRegisteredToday=false, CaseName = "PodRegToday-MPRN_Dereg_ElecDevices_NoGas", Output = OutputType.NotExecuted},
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = false, IsMPRNDeregistered = true, GasAccount = true, GasAccountHasDevices = false, IsGPRNDeregistered = false, IsPodRegisteredToday=false, CaseName = "PodRegToday-MPRN_Dereg_NoDevices_HasBothConn", Output = OutputType.NotExecuted },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = true, IsMPRNDeregistered = true, GasAccount = true, GasAccountHasDevices = true, IsGPRNDeregistered = false, IsPodRegisteredToday=false, CaseName = "PodRegToday-MPRN_Dereg_HasBothDevice", Output = OutputType.Passed },

                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = false, IsMPRNDeregistered = false, GasAccount = false, GasAccountHasDevices = false, IsGPRNDeregistered = true, IsPodRegisteredToday = false, CaseName = "PodRegToday-GPRN_Dereg_NoDevices_NoGas", Output = OutputType.NotExecuted },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = true, IsMPRNDeregistered = false, GasAccount = false, GasAccountHasDevices = false, IsGPRNDeregistered = true, IsPodRegisteredToday=false, CaseName = "PodRegToday-GPRN_Dereg_ElecDevices_NoGas", Output = OutputType.Passed},
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = false, IsMPRNDeregistered = false, GasAccount = true, GasAccountHasDevices = false, IsGPRNDeregistered = true, IsPodRegisteredToday=false, CaseName = "PodRegToday-GPRN_Dereg_NoDevices_HasBothConn", Output = OutputType.NotExecuted },
                new CaseModel { ValidateNewPremisePodNotRegisteredToday = true, ElectricAccount = true, ElectricAccountHasDevices = true, IsMPRNDeregistered = false, GasAccount = true, GasAccountHasDevices = true, IsGPRNDeregistered = true, IsPodRegisteredToday=false, CaseName = "PodRegToday-GPRN_Dereg_HasBothDevice", Output = OutputType.Passed },
			};

            foreach (var caseItem in cases)
            {
                var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
                {
                    Output = caseItem.Output,
                    MovingHouseValidationType =
                            MovingHouseValidationType.PodNotRegisteredEarlierToday
                };
                yield return new TestCaseData(caseItem).SetName(caseItem.CaseName).Returns(movingHouseRulesValidationResult);
            }
        }

        [TestCaseSource(nameof(CanResolveCases))]
        public async Task<MovingHouseRulesValidationResult> CanResolve(CaseModel caseModel)
        {
            return await Context
                .WithElectricityAccount(caseModel.ElectricAccount, caseModel.ElectricAccountHasDevices)
                .WithGasAccount(caseModel.GasAccount, caseModel.GasAccountHasDevices)
                .WithValidateNewPremisePodNotRegisteredToday(caseModel.ValidateNewPremisePodNotRegisteredToday)
                .WithIsMPRNDeregistered(caseModel.IsMPRNDeregistered)
                .WithIsGPRNDeregistered(caseModel.IsGPRNDeregistered)
				.WithIsPodRegisteredToday(caseModel.IsPodRegisteredToday)
                .Sut
                .Resolve(Context.Query);
        }

        public class TestContext : UnitTestContext<PodNotRegisteredEarlierTodayValidator>
        {
            private bool _validateNewPremisePodNotRegisteredToday;
            private bool _addElectricity;
            private bool _electricAccountHasDevices;
            private bool _addGas;
            private bool _gasAccountHasDevices;
            private bool _isMPRNDeregistered;
            private bool _isGPRNDeregistered;
			private bool _isPodRegisteredToday;

            public MovingHouseValidationQuery Query { get; private set; }

            public TestContext WithValidateNewPremisePodNotRegisteredToday(bool validateNewPremisePodNotRegisteredToday)
            {
                _validateNewPremisePodNotRegisteredToday = validateNewPremisePodNotRegisteredToday;
                return this;
            }

            public TestContext WithIsMPRNDeregistered(bool isMPRNDeregistered)
            {
                _isMPRNDeregistered = isMPRNDeregistered;
                return this;
            }

            public TestContext WithIsGPRNDeregistered(bool isGPRNDeregistered)
            {
	            _isGPRNDeregistered = isGPRNDeregistered;
	            return this;
            }

			public TestContext WithElectricityAccount(bool addElectricity, bool electricAccountHasDevices)
            {
                _addElectricity = addElectricity;
                _electricAccountHasDevices = electricAccountHasDevices;
                return this;
            }

            public TestContext WithIsPodRegisteredToday(bool isPodRegisteredToday)
            {
                _isPodRegisteredToday = isPodRegisteredToday;
                return this;
            }

            public TestContext WithGasAccount(bool addGas, bool gasAccountHasDevices)
            {
                _addGas = addGas;
                _gasAccountHasDevices = gasAccountHasDevices;
                return this;
            }

            protected override PodNotRegisteredEarlierTodayValidator BuildSut(AutoMocker autoMocker)
            {
                var domainFacade = new DomainFacade();
                var nowDateTime = DateTime.Now;
                if (_isPodRegisteredToday)
                {
                    domainFacade.ModelsBuilder.Customize<MeterReadingInfo>(composer =>
                        composer.With(x => x.MeterReadingReasonID, "06")
                            .With(x => x.ReadingDateTime, nowDateTime));
                }
                autoMocker.Use<ICompositeRuleValidatorOutputTypeResolver>(new CompositeRuleValidatorOutputResolver());
                domainFacade.SetUpMocker(autoMocker);
                var cfg = new AppUserConfigurator(domainFacade);
                if (_addElectricity)
                {
                    cfg.AddElectricityAccount(configureDefaultDevice: _electricAccountHasDevices);
                }

                if (_addGas)
                {
                    cfg.AddGasAccount(configureDefaultDevice: _gasAccountHasDevices);
                }

                cfg.Execute();

                var electricityAccount = cfg.ElectricityAccount()?.Model;
                var gasAccount = cfg.GasAccount()?.Model;

                var newPremiseElectricity = cfg.ElectricityAccount()?.NewPremise.ElectricityPrn.ToString();
                var newPremiseGas = cfg.GasAccount()?.NewPremise.GasPrn.ToString();

                Query = Fixture.Build<MovingHouseValidationQuery>()
                    .With(x => x.ElectricityAccountNumber, electricityAccount?.AccountNumber)
                    .With(x => x.GasAccountNumber, gasAccount?.AccountNumber)
                    .With(x => x.ValidateNewPremisePodNotRegisteredToday, _validateNewPremisePodNotRegisteredToday)
                    .With(x => x.IsMPRNDeregistered, _isMPRNDeregistered)
                    .With(x => x.IsGPRNDeregistered, _isGPRNDeregistered)
					.With(x => x.NewMPRN, newPremiseElectricity)
                    .With(x => x.NewGPRN, newPremiseGas)
                    .With(x => x.MoveOutDate, nowDateTime)
                    .Without(x => x.MovingHouseType)
                    .Create();

                return base.BuildSut(autoMocker);
            }
        }
    }
}