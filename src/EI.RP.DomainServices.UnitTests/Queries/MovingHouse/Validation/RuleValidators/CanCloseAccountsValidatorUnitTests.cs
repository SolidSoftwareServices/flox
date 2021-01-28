using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.Contracts.CanCloseAccount;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.Validation.RuleValidators
{
    [TestFixture]
    internal class CanCloseAccountsValidatorUnitTests : UnitTestFixture<
        CanCloseAccountsValidatorUnitTests.TestContext,
        CanCloseAccountsValidator>
    {
        internal class CaseModel
        {
            public bool ValidateCanCloseAccounts { get; set; }
            public bool ElectricAccount { get; set; }
            public DateTime? MoveOutDate { get; set; }
            public bool GasAccount { get; set; }
            public bool CanCloseQueryResult { get; set; }
            public OutputType Output { get; set; }

            public override string ToString()
            {
	            return $"{nameof(ValidateCanCloseAccounts)}: {ValidateCanCloseAccounts}, {nameof(ElectricAccount)}: {ElectricAccount}, {nameof(MoveOutDate)}: {MoveOutDate}, {nameof(GasAccount)}: {GasAccount}, {nameof(CanCloseQueryResult)}: {CanCloseQueryResult}, {nameof(Output)}: {Output}";
            }
        }

        public static IEnumerable<TestCaseData> CanResolveCases()
        {
            var cases = new[]
            {
                new CaseModel { ValidateCanCloseAccounts = false, ElectricAccount = true, MoveOutDate = DateTime.Now, GasAccount = false, CanCloseQueryResult = true, Output = OutputType.NotExecuted },
                new CaseModel { ValidateCanCloseAccounts = true, ElectricAccount = true, MoveOutDate = DateTime.Now, GasAccount = false, CanCloseQueryResult = true, Output = OutputType.Passed },
                new CaseModel { ValidateCanCloseAccounts = true, ElectricAccount = true, MoveOutDate = DateTime.Now, GasAccount = false, CanCloseQueryResult = false, Output = OutputType.Failed },
                new CaseModel { ValidateCanCloseAccounts = true, ElectricAccount = true, MoveOutDate = null, GasAccount = false, CanCloseQueryResult = true, Output = OutputType.Failed },

                new CaseModel { ValidateCanCloseAccounts = false, ElectricAccount = false, MoveOutDate = DateTime.Now, GasAccount = true, CanCloseQueryResult = true, Output = OutputType.NotExecuted },
                new CaseModel { ValidateCanCloseAccounts = true, ElectricAccount = false, MoveOutDate = DateTime.Now, GasAccount = true, CanCloseQueryResult = true, Output = OutputType.Passed },
                new CaseModel { ValidateCanCloseAccounts = true, ElectricAccount = false, MoveOutDate = DateTime.Now, GasAccount = true, CanCloseQueryResult = false, Output = OutputType.Failed },
                new CaseModel { ValidateCanCloseAccounts = true, ElectricAccount = false, MoveOutDate = null, GasAccount = true, CanCloseQueryResult = true, Output = OutputType.Failed },

                new CaseModel { ValidateCanCloseAccounts = false, ElectricAccount = false, MoveOutDate = DateTime.Now, GasAccount = false, CanCloseQueryResult = true, Output = OutputType.NotExecuted },
            };

            foreach (var caseItem in cases)
            {
                var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
                {
                    Output = caseItem.Output,
                    MovingHouseValidationType =
                            MovingHouseValidationType.CanCloseAccounts
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
                .WithValidateCanCloseAccounts(caseModel.ValidateCanCloseAccounts)
                .WithElectricityAccount(caseModel.ElectricAccount)
                .WithGasAccount(caseModel.GasAccount)
                .WithMoveOutDate(caseModel.MoveOutDate)
                .WithCanCloseQueryResult(caseModel.CanCloseQueryResult)
                .Sut
                .Resolve(Context.Query);
        }

        public class TestContext : UnitTestContext<CanCloseAccountsValidator>
        {
            private bool _addElectricity;
            private bool _addGas;
            private bool _validateCanCloseAccounts;
            private DateTime? _moveOutDate;
            private bool _canCloseQueryResult;

            public MovingHouseValidationQuery Query { get; private set; }

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

            public TestContext WithMoveOutDate(DateTime? moveOutDate)
            {
                _moveOutDate = moveOutDate;
                return this;
            }

            public TestContext WithValidateCanCloseAccounts(bool validateCanCloseAccounts)
            {
                _validateCanCloseAccounts = validateCanCloseAccounts;
                return this;
            }

            public TestContext WithCanCloseQueryResult(bool canCloseQueryResult)
            {
                _canCloseQueryResult = canCloseQueryResult;
                return this;
            }

            protected override CanCloseAccountsValidator BuildSut(AutoMocker autoMocker)
            {
                var domainFacade = new DomainFacade();
                domainFacade.SetUpMocker(autoMocker);
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

                var query = new CanCloseAccountQuery
                {
                    ElectricityAccountNumber = electricityAccount?.AccountNumber,
                    GasAccountNumber = gasAccount?.AccountNumber,
                    ClosingDate = _moveOutDate.GetValueOrDefault()
                };

                domainFacade.QueryResolver.ExpectQuery<CanCloseAccountQuery, AccountsCloseableInfo>(query, new AccountsCloseableInfo()
                {
                    CanClose = _canCloseQueryResult
                }.ToOneItemArray());

                Query = Fixture.Build<MovingHouseValidationQuery>()
                    .With(x => x.ElectricityAccountNumber, electricityAccount?.AccountNumber)
                    .With(x => x.GasAccountNumber, gasAccount?.AccountNumber)
                    .With(x => x.ValidateCanCloseAccounts, _validateCanCloseAccounts)
                    .With(x => x.MoveOutDate, _moveOutDate)
                    .Without(x => x.MovingHouseType)
                    .Create();

                return base.BuildSut(autoMocker);
            }
        }
    }
}