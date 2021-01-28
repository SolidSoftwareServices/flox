using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse;
using EI.RP.DomainServices.InternalShared.ContractSales;
using Ei.Rp.DomainErrors;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.TestServices;
using Moq.AutoMock;
using NUnit.Framework;
using Moq;

namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.Validation.RuleValidators
{
    [TestFixture]
    internal class ContractSaleChecksValidatorUnitTests : UnitTestFixture<
        ContractSaleChecksValidatorUnitTests.TestContext,
        ContractSaleChecksValidator>
    {
        internal class CaseModel
        {
            public bool ValidateContractSaleChecks { get; set; }
            public bool InitiatedFromElectricity { get; set; }
            public bool ElectricAccount { get; set; }
            public MovingHouseType MovingHouseType { get; set; }
            public bool GasAccount { get; set; }
            public bool SaleChecksResult { get; set; }
            public OutputType Output { get; set; }
            public string TestName { get; set; }
        }

        public static IEnumerable<TestCaseData> CanResolveCases()
        {
            var cases = new[]
            {
                new CaseModel { ValidateContractSaleChecks = false, InitiatedFromElectricity = true, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricity, GasAccount = false, SaleChecksResult = true, Output = OutputType.NotExecuted, TestName="MoveElectricityValidateContractSaleChecksFlagNotSet" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = true, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricity, GasAccount = false, SaleChecksResult = true, Output = OutputType.Passed, TestName="MoveElectricitySaleChecksResultTrue" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = true, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricity, GasAccount = false, SaleChecksResult = false, Output = OutputType.Failed, TestName="MoveElectricitySaleChecksResultFalse"},

                new CaseModel { ValidateContractSaleChecks = false, InitiatedFromElectricity = true, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricityAndAddGas, GasAccount = false, SaleChecksResult = true, Output = OutputType.NotExecuted, TestName="MoveElectricityAndAddGasValidateContractSaleChecksFlagNotSet" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = true, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricityAndAddGas, GasAccount = false, SaleChecksResult = true, Output = OutputType.Passed, TestName="MoveElectricityAndAddGasSaleChecksResultTrue" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = true, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricityAndAddGas, GasAccount = false, SaleChecksResult = false, Output = OutputType.Failed, TestName="MoveElectricityAndAddGasSaleChecksResultFalse" },

                new CaseModel { ValidateContractSaleChecks = false, InitiatedFromElectricity = false, ElectricAccount = false, MovingHouseType = MovingHouseType.MoveGas, GasAccount = true, SaleChecksResult = true, Output = OutputType.NotExecuted, TestName="MoveGasValidateContractSaleChecksFlagNotSet" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = false, ElectricAccount = false, MovingHouseType = MovingHouseType.MoveGas, GasAccount = true, SaleChecksResult = true, Output = OutputType.Passed, TestName="MoveGasSaleChecksResultTrue" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = false, ElectricAccount = false, MovingHouseType = MovingHouseType.MoveGas, GasAccount = true, SaleChecksResult = false, Output = OutputType.Failed, TestName="MoveGasSaleChecksResultFalse" },

                new CaseModel { ValidateContractSaleChecks = false, InitiatedFromElectricity = false, ElectricAccount = false, MovingHouseType = MovingHouseType.MoveGasAndAddElectricity, GasAccount = true, SaleChecksResult = true, Output = OutputType.NotExecuted, TestName="MoveGasAndAddElectricityValidateContractSaleChecksFlagNotSet" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = false, ElectricAccount = false, MovingHouseType = MovingHouseType.MoveGasAndAddElectricity, GasAccount = true, SaleChecksResult = true, Output = OutputType.Passed, TestName="MoveGasAndAddElectricitySaleChecksResultTrue" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = false, ElectricAccount = false, MovingHouseType = MovingHouseType.MoveGasAndAddElectricity, GasAccount = true, SaleChecksResult = false, Output = OutputType.Failed, TestName="MoveGasAndAddElectricitySaleChecksResultFalse" },

                new CaseModel { ValidateContractSaleChecks = false, InitiatedFromElectricity = true, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricityAndGas, GasAccount = true, SaleChecksResult = true, Output = OutputType.NotExecuted, TestName="MoveElectricityAndGasGasAccountTrueValidateContractSaleChecksFlagNotSet" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = true, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricityAndGas, GasAccount = true, SaleChecksResult = true, Output = OutputType.Passed, TestName="MoveElectricityAndGasGasAccountTrueSaleChecksResultTrue" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = true, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricityAndGas, GasAccount = true, SaleChecksResult = false, Output = OutputType.Failed, TestName="MoveElectricityAndGasGasAccountTrueSaleChecksResultFalse" },

                new CaseModel { ValidateContractSaleChecks = false, InitiatedFromElectricity = true, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricity, GasAccount = true, SaleChecksResult = true, Output = OutputType.NotExecuted, TestName="MoveElectricityGasAccountTrueValidateContractSaleChecksFlagNotSet" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = true, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricity, GasAccount = true, SaleChecksResult = true, Output = OutputType.Passed, TestName="MoveElectricityGasAccountTrueSaleChecksResultTrue" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = true, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricity, GasAccount = true, SaleChecksResult = false, Output = OutputType.Failed, TestName="MoveElectricityGasAccountTrueSaleChecksResultFalse" },

                new CaseModel { ValidateContractSaleChecks = false, InitiatedFromElectricity = false, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricityAndGas, GasAccount = true, SaleChecksResult = true, Output = OutputType.NotExecuted, TestName="InitiatedFromGasValidateContractSaleChecksFlagNotSet" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = false, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricityAndGas, GasAccount = true, SaleChecksResult = true, Output = OutputType.Passed, TestName="InitiatedFromGasMoveElectricityAndGasSaleChecksResultTrue" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = false, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveElectricityAndGas, GasAccount = true, SaleChecksResult = false, Output = OutputType.Failed, TestName="InitiatedFromGasMoveElectricityAndGasSaleChecksResultFalse" },

                new CaseModel { ValidateContractSaleChecks = false, InitiatedFromElectricity = false, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveGas, GasAccount = true, SaleChecksResult = true, Output = OutputType.NotExecuted, TestName="InitiatedFromGasMoveGasValidateContractSaleChecksFlagNotSet" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = false, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveGas, GasAccount = true, SaleChecksResult = true, Output = OutputType.Passed, TestName="InitiatedFromGasMoveGasSaleChecksResultTrue" },
                new CaseModel { ValidateContractSaleChecks = true, InitiatedFromElectricity = false, ElectricAccount = true, MovingHouseType = MovingHouseType.MoveGas, GasAccount = true, SaleChecksResult = false, Output = OutputType.Failed, TestName="InitiatedFromGasMoveGasSaleChecksResultFalse" },

            };

            foreach (var caseItem in cases)
            {
                var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
                {
                    Output = caseItem.Output,
                    MovingHouseValidationType =
                            MovingHouseValidationType.IsContractSaleChecksOk
                };
                yield return new TestCaseData(caseItem).Returns(movingHouseRulesValidationResult).SetName(caseItem.TestName);
            }
        }

        [TestCaseSource(nameof(CanResolveCases))]
        public async Task<MovingHouseRulesValidationResult> CanResolve(CaseModel caseModel)
        {
            return await Context
                .WithValidateContractSaleChecks(caseModel.ValidateContractSaleChecks)
                .WithElectricityAccount(caseModel.ElectricAccount)
                .WithGasAccount(caseModel.GasAccount)
                .WithMovingHouseType(caseModel.MovingHouseType)
                .WithInitiatedFromElectricity(caseModel.InitiatedFromElectricity)
                .WithSaleChecksResult(caseModel.SaleChecksResult)
                .Sut
                .Resolve(Context.Query);
        }

        public class TestContext : UnitTestContext<ContractSaleChecksValidator>
        {
            private bool _addElectricity;
            private bool _addGas;
            private bool _validateContractSaleChecks;
            private MovingHouseType _movingHouseType;
            private bool _saleChecksResult;
            private bool _initiatedFromElectricity;
            private string _initiatedFromAccountNumber;
            private ClientAccountType _clientAccountType;

            public MovingHouseValidationQuery Query { get; private set; }

            public TestContext WithInitiatedFromElectricity(bool initiatedFromElectricity)
            {
                _initiatedFromElectricity = initiatedFromElectricity;
                return this;
            }

            public TestContext WithElectricityAccount(bool addElectricity)
            {
                _addElectricity = addElectricity;
                return this;
            }

            public TestContext WithGasAccount(bool addGas)
            {
                _addGas = addGas;
                return this;
            }

            public TestContext WithMovingHouseType(MovingHouseType movingHouseType)
            {
                _movingHouseType = movingHouseType;
                return this;
            }

            public TestContext WithValidateContractSaleChecks(bool validateContractSaleChecks)
            {
                _validateContractSaleChecks = validateContractSaleChecks;
                return this;
            }

            public TestContext WithSaleChecksResult(bool saleChecksResult)
            {
                _saleChecksResult = saleChecksResult;
                return this;
            }

            protected override ContractSaleChecksValidator BuildSut(AutoMocker autoMocker)
            {
                var domainFacade = new DomainFacade();
                var cfg = new AppUserConfigurator(domainFacade);
                if (_addElectricity)
                {
                    cfg.AddElectricityAccount()
                        .WithElectricity24HrsDevices();
                }

                if (_addGas)
                {
                    cfg.AddGasAccount()
                        .WithGasDevice();
                }

                cfg.Execute();

                var electricityAccount = cfg.ElectricityAccount()?.Model;
                var gasAccount = cfg.GasAccount()?.Model;

                _initiatedFromAccountNumber = _initiatedFromElectricity ? electricityAccount?.AccountNumber : gasAccount?.AccountNumber;
                _clientAccountType = _initiatedFromElectricity ? ClientAccountType.Electricity : ClientAccountType.Gas;

                var query = new AccountInfoQuery
                {
                    AccountNumber = _initiatedFromAccountNumber
                };

                domainFacade.QueryResolver.ExpectQuery<AccountInfoQuery, AccountInfo>(query, new AccountInfo
                {
                    ClientAccountType = _clientAccountType
                }.ToOneItemArray());

                var cmd = new MoveHouse(
                    electricityAccount?.AccountNumber,
                    gasAccount?.AccountNumber,
                    _movingHouseType,
                    null,
                    _clientAccountType);

                var contractSaleCommandMock = autoMocker.GetMock<IContractSaleCommand>();

                if (_saleChecksResult)
                {
                    contractSaleCommandMock
                        .Setup(x => x.ResolveContractSaleChecks(cmd, false))
                        .ReturnsAsync(new ContractSaleDto());
                }
                else
                {
                    contractSaleCommandMock
                        .Setup(x => x.ResolveContractSaleChecks(cmd, false))
                        .Throws(new DomainException(ResidentialDomainError.ContractErrorPreventTheContactFromBeingSubmitted));
                }

                domainFacade.SetUpMocker(autoMocker);

                Query = Fixture.Build<MovingHouseValidationQuery>()
                    .With(x => x.ElectricityAccountNumber, electricityAccount?.AccountNumber)
                    .With(x => x.GasAccountNumber, gasAccount?.AccountNumber)
                    .With(x => x.ValidateContractSaleChecks, _validateContractSaleChecks)
                    .With(x => x.MovingHouseType, _movingHouseType)
                    .With(x => x.InitiatedFromAccountNumber, _initiatedFromAccountNumber)
                    .Create();

                return base.BuildSut(autoMocker);
            }
        }
    }
}