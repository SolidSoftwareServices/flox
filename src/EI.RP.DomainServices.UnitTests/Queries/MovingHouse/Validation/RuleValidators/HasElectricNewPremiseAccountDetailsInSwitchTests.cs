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

namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.Validation.RuleValidators
{
    [TestFixture]
    internal class HasElectricNewPremiseAccountDetailsInSwitchTests : UnitTestFixture<
        HasElectricNewPremiseAccountDetailsInSwitchTests.TestContext,
        HasElectricNewPremiseAccountDetailsInSwitchValidator>
    {
        internal class CaseModel
        {
            public bool ValidateElectricNewPremiseAddressInSwitch { get; set; }
            public bool IsNewMPRNAddressInSwitch { get; set; }
            public bool IsNewAcquisitionElectricity { get; set; }
            public OutputType Output { get; set; }

            public override string ToString()
            {
	            return $"{nameof(ValidateElectricNewPremiseAddressInSwitch)}: {ValidateElectricNewPremiseAddressInSwitch}, {nameof(IsNewMPRNAddressInSwitch)}: {IsNewMPRNAddressInSwitch}, {nameof(IsNewAcquisitionElectricity)}: {IsNewAcquisitionElectricity}, {nameof(Output)}: {Output}";
            }
        }

        public static IEnumerable<TestCaseData> CanResolveCases()
        {
            var cases = new CaseModel[]
            {
                new CaseModel { ValidateElectricNewPremiseAddressInSwitch = true, IsNewAcquisitionElectricity = true, IsNewMPRNAddressInSwitch = true, Output = OutputType.Passed },
                new CaseModel { ValidateElectricNewPremiseAddressInSwitch = true, IsNewAcquisitionElectricity = true, IsNewMPRNAddressInSwitch = false, Output = OutputType.Failed },
                new CaseModel { ValidateElectricNewPremiseAddressInSwitch = false, IsNewAcquisitionElectricity = true, IsNewMPRNAddressInSwitch = false, Output = OutputType.NotExecuted },

                new CaseModel { ValidateElectricNewPremiseAddressInSwitch = true, IsNewAcquisitionElectricity = false, IsNewMPRNAddressInSwitch = true, Output = OutputType.Passed },
                new CaseModel { ValidateElectricNewPremiseAddressInSwitch = true, IsNewAcquisitionElectricity = false, IsNewMPRNAddressInSwitch = false, Output = OutputType.Passed },
                new CaseModel { ValidateElectricNewPremiseAddressInSwitch = false, IsNewAcquisitionElectricity = false, IsNewMPRNAddressInSwitch = false, Output = OutputType.NotExecuted },

            };

            foreach (var caseItem in cases)
            {
                var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
                {
                    Output = caseItem.Output,
                    MovingHouseValidationType =
                            MovingHouseValidationType.HasElectricNewPremiseAccountDetailsInSwitch
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
                .WithValidateElectricNewPremiseAddressInSwitch(caseModel.ValidateElectricNewPremiseAddressInSwitch)
                .WithNewMPRNAddressInSwitch(caseModel.IsNewMPRNAddressInSwitch)
                .WithIsNewAcquisitionElectricity(caseModel.IsNewAcquisitionElectricity)
                .Sut
                .Resolve(Context.Query);
        }

        public class TestContext : UnitTestContext<HasElectricNewPremiseAccountDetailsInSwitchValidator>
        {
            private bool _ValidateElectricNewPremiseAddressInSwitch;
            private bool _IsNewMPRNAddressInSwitch;
            private bool _IsNewAcquisitionElectricity;

            public MovingHouseValidationQuery Query { get; private set; }

            public TestContext WithValidateElectricNewPremiseAddressInSwitch(bool validateElectricNewPremiseAddressInSwitch)
            {
                _ValidateElectricNewPremiseAddressInSwitch = validateElectricNewPremiseAddressInSwitch;
                return this;
            }

            public TestContext WithNewMPRNAddressInSwitch(bool isNewMPRNAddressInSwitch)
            {
                _IsNewMPRNAddressInSwitch = isNewMPRNAddressInSwitch;
                return this;
            }

            public TestContext WithIsNewAcquisitionElectricity(bool isNewAcquisitionElectricity)
            {
                _IsNewAcquisitionElectricity = isNewAcquisitionElectricity;
                return this;
            }

            protected override HasElectricNewPremiseAccountDetailsInSwitchValidator BuildSut(AutoMocker autoMocker)
            {
                var domainFacade = new DomainFacade();
                domainFacade.SetUpMocker(autoMocker);
                var cfg = new AppUserConfigurator(domainFacade);
                cfg.AddElectricityAccount(isNewMPRNAddressInSwitch : _IsNewMPRNAddressInSwitch);
                cfg.Execute();

                var electricityAccount = cfg.ElectricityAccount()?.Model;
                var newPremiseElectricity = cfg.ElectricityAccount()?.NewPremise.ElectricityPrn.ToString();

                Query = Fixture.Build<MovingHouseValidationQuery>()
                    .With(x => x.ElectricityAccountNumber, electricityAccount?.AccountNumber)
                    .With(x => x.ValidateElectricNewPremiseAddressInSwitch, _ValidateElectricNewPremiseAddressInSwitch)
                    .With(x => x.NewMPRN, newPremiseElectricity)
                    .With(x => x.IsNewAcquisitionElectricity, _IsNewAcquisitionElectricity)
                    .With(x => x.IsMPRNAddressInSwitch, _IsNewMPRNAddressInSwitch)
                    .Without(x => x.MovingHouseType)
                    .Create();

                return base.BuildSut(autoMocker);
            }
        }
    }
}