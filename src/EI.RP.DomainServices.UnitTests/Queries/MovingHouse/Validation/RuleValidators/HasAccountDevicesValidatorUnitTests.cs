using AutoFixture;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
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
    internal class HasAccountDevicesValidatorUnitTests : UnitTestFixture<
        HasAccountDevicesValidatorUnitTests.TestContext,
        HasAccountDevicesValidator>
    {
        internal class CaseModel
        {
            public bool ValidateHasAccountDevices { get; set; }
            public bool ElectricAccount { get; set; }
            public bool ElectricAccountHasDevices { get; set; }
            public bool GasAccount { get; set; }
            public bool GasAccountHasDevices { get; set; }
            public OutputType Output { get; set; }

            public override string ToString()
            {
	            return $"{nameof(ValidateHasAccountDevices)}: {ValidateHasAccountDevices}, {nameof(ElectricAccount)}: {ElectricAccount}, {nameof(ElectricAccountHasDevices)}: {ElectricAccountHasDevices}, {nameof(GasAccount)}: {GasAccount}, {nameof(GasAccountHasDevices)}: {GasAccountHasDevices}, {nameof(Output)}: {Output}";
            }
        }

        public static IEnumerable<TestCaseData> CanResolveCases()
        {
            var cases = new CaseModel[]
            {
                new CaseModel { ValidateHasAccountDevices = true, ElectricAccount = true, ElectricAccountHasDevices = true, GasAccount = false, GasAccountHasDevices = false, Output = OutputType.Passed },
                new CaseModel { ValidateHasAccountDevices = true, ElectricAccount = true, ElectricAccountHasDevices = false, GasAccount = false, GasAccountHasDevices = false, Output = OutputType.Failed },
                new CaseModel { ValidateHasAccountDevices = false, ElectricAccount = true, ElectricAccountHasDevices = true, GasAccount = false, GasAccountHasDevices = false, Output = OutputType.NotExecuted },
                new CaseModel { ValidateHasAccountDevices = true, ElectricAccount = false, ElectricAccountHasDevices = false, GasAccount = true, GasAccountHasDevices = true, Output = OutputType.Passed },
                new CaseModel { ValidateHasAccountDevices = true, ElectricAccount = false, ElectricAccountHasDevices = false, GasAccount = true, GasAccountHasDevices = false, Output = OutputType.Failed },
                new CaseModel { ValidateHasAccountDevices = false, ElectricAccount = false, ElectricAccountHasDevices = false, GasAccount = true, GasAccountHasDevices = true, Output = OutputType.NotExecuted },
                new CaseModel { ValidateHasAccountDevices = true, ElectricAccount = true, ElectricAccountHasDevices = true, GasAccount = true, GasAccountHasDevices = true, Output = OutputType.Passed },
                new CaseModel { ValidateHasAccountDevices = true, ElectricAccount = true, ElectricAccountHasDevices = false, GasAccount = true, GasAccountHasDevices = false, Output = OutputType.Failed },
                new CaseModel { ValidateHasAccountDevices = true, ElectricAccount = false, ElectricAccountHasDevices = false, GasAccount = false, GasAccountHasDevices = false, Output = OutputType.Failed },
                new CaseModel { ValidateHasAccountDevices = false, ElectricAccount = true, ElectricAccountHasDevices = true, GasAccount = true, GasAccountHasDevices = true, Output = OutputType.NotExecuted },
            };

            foreach (var caseItem in cases)
            {
                var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
                {
                    Output = caseItem.Output,
                    MovingHouseValidationType =
                            MovingHouseValidationType.HasAccountDevices
                };
                yield return new TestCaseData(caseItem)
	                .SetName(caseItem.ToString())
	                .Returns(movingHouseRulesValidationResult);
            }
        }

        [TestCaseSource(nameof(CanResolveCases))]
        public async Task<MovingHouseRulesValidationResult> CanResolve(CaseModel caseModel)
        {
            return await Context
                .WithValidateHasAccountDevices(caseModel.ValidateHasAccountDevices)
                .WithElectricityAccount(caseModel.ElectricAccount, caseModel.ElectricAccountHasDevices)
                .WithGasAccount(caseModel.GasAccount, caseModel.GasAccountHasDevices)
                .Sut
                .Resolve(Context.Query);
        }

        public class TestContext : UnitTestContext<HasAccountDevicesValidator>
        {
            private bool _ValidateHasAccountDevices;
            private bool _addElectricity;
            private bool _electricAccountHasDevices;
            private bool _addGas;
            private bool _gasAccountHasDevices;

            public MovingHouseValidationQuery Query { get; private set; }

            public TestContext WithValidateHasAccountDevices(bool validateHasAccountDevices)
            {
                _ValidateHasAccountDevices = validateHasAccountDevices;
                return this;
            }

            public TestContext WithElectricityAccount(bool addElectricity, bool electricAccountHasDevices)
            {
                _addElectricity = addElectricity;
                _electricAccountHasDevices = electricAccountHasDevices;
                return this;
            }

            public TestContext WithGasAccount(bool addGas, bool gasAccountHasDevices)
            {
                _addGas = addGas;
                _gasAccountHasDevices = gasAccountHasDevices;
                return this;
            }

            protected override HasAccountDevicesValidator BuildSut(AutoMocker autoMocker)
            {
                var domainFacade = new DomainFacade();
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

                Query = Fixture.Build<MovingHouseValidationQuery>()
                    .With(x => x.ValidateHasAccountDevices, _ValidateHasAccountDevices)
                    .With(x => x.ElectricityAccountNumber, electricityAccount?.AccountNumber)
                    .With(x => x.GasAccountNumber, gasAccount?.AccountNumber)
                    .Without(x => x.MovingHouseType)
                    .Create();

                return base.BuildSut(autoMocker);
            }
        }
    }
}