using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.Validation.RuleValidators
{
	[TestFixture]
	internal class NewPremisePointReferenceNumbersAreValidValidatorUnitTests : UnitTestFixture<
        NewPremisePointReferenceNumbersAreValidValidatorUnitTests.TestContext,
        NewPremisePointReferenceNumbersAreValidValidator>
	{
        internal class CaseModel
        {
            public bool ValidateNewPremise { get; set; }
            public string NewMPRN { get; set; }
            public string NewGPRN { get; set; }
            public MovingHouseType MovingHouseType { get; set; }
            public OutputType Output { get; set; }

            public override string ToString()
            {
	            return $"{nameof(ValidateNewPremise)}: {ValidateNewPremise}, {nameof(NewMPRN)}: {NewMPRN}, {nameof(NewGPRN)}: {NewGPRN}, {nameof(MovingHouseType)}: {MovingHouseType}, {nameof(Output)}: {Output}";
            }
        }

        public static IEnumerable<TestCaseData> CanResolveCases()
        {
            var fixture = new Fixture().CustomizeDomainTypeBuilders();
            var testNewMPRN = (string)fixture.Create<ElectricityPointReferenceNumber>();
            var testNewGRPN = (string)fixture.Create<GasPointReferenceNumber>();

            var cases = new CaseModel[]
            {
                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveElectricity, NewMPRN = testNewMPRN, Output = OutputType.Passed },
                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveElectricity, NewMPRN = null, Output = OutputType.Failed },
                new CaseModel { ValidateNewPremise = false, MovingHouseType = MovingHouseType.MoveElectricity, NewMPRN = null, Output = OutputType.NotExecuted },

                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveElectricityAndCloseGas, NewMPRN = testNewMPRN, NewGPRN = null, Output = OutputType.Passed },
                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveElectricityAndCloseGas, NewMPRN = null, NewGPRN = null, Output = OutputType.Failed },
                new CaseModel { ValidateNewPremise = false, MovingHouseType = MovingHouseType.MoveElectricityAndCloseGas, NewMPRN = null, NewGPRN = null, Output = OutputType.NotExecuted },

                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveGas, NewMPRN = null, NewGPRN = testNewGRPN, Output = OutputType.Passed },
                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveGas, NewMPRN = null, NewGPRN = null, Output = OutputType.Failed },
                new CaseModel { ValidateNewPremise = false, MovingHouseType = MovingHouseType.MoveGas, NewMPRN = null, NewGPRN = null, Output = OutputType.NotExecuted },

                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveGasAndAddElectricity, NewMPRN = testNewMPRN, NewGPRN = testNewGRPN, Output = OutputType.Passed },
                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveGasAndAddElectricity, NewMPRN = null, NewGPRN = testNewGRPN, Output = OutputType.Failed },
                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveGasAndAddElectricity, NewMPRN = testNewMPRN, NewGPRN = null, Output = OutputType.Failed },
                new CaseModel { ValidateNewPremise = false, MovingHouseType = MovingHouseType.MoveGasAndAddElectricity, NewMPRN = testNewMPRN, NewGPRN = null, Output = OutputType.NotExecuted },

                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveElectricityAndAddGas, NewMPRN = testNewMPRN, NewGPRN = testNewGRPN, Output = OutputType.Passed },
                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveElectricityAndAddGas, NewMPRN = testNewMPRN, NewGPRN = null, Output = OutputType.Failed },
                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveElectricityAndAddGas, NewMPRN = null, NewGPRN = testNewGRPN, Output = OutputType.Failed },
                new CaseModel { ValidateNewPremise = false, MovingHouseType = MovingHouseType.MoveElectricityAndAddGas, NewMPRN = null, NewGPRN = testNewGRPN, Output = OutputType.NotExecuted },

                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveElectricityAndGas, NewMPRN = testNewMPRN, NewGPRN = testNewGRPN, Output = OutputType.Passed },
                new CaseModel { ValidateNewPremise = true, MovingHouseType = MovingHouseType.MoveElectricityAndGas, NewMPRN = testNewMPRN, NewGPRN = null, Output = OutputType.Failed },
                new CaseModel { ValidateNewPremise = false, MovingHouseType = MovingHouseType.MoveElectricityAndGas, NewMPRN = testNewMPRN, NewGPRN = null, Output = OutputType.NotExecuted },
            };

            foreach (var caseItem in cases) {               
                var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
                {
                    Output = caseItem.Output,
                    MovingHouseValidationType =
                            MovingHouseValidationType.NewPremisePointReferenceNumbersAreValid
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
                .WithValidateNewPremise(caseModel.ValidateNewPremise)
                .WithMovingHouseType(caseModel.MovingHouseType)
                .WithNewMPRN(caseModel.NewMPRN)
                .WithNewGPRN(caseModel.NewGPRN)
                .Sut
                .Resolve(Context.Query);
        }

        public class TestContext : UnitTestContext<NewPremisePointReferenceNumbersAreValidValidator>
        {
            private bool _ValidateNewPremise;
            private string _NewMPRN;
            private string _NewGPRN;
            private MovingHouseType _MovingHouseType;

            public MovingHouseValidationQuery Query { get; private set; }

            public TestContext WithValidateNewPremise(bool validateNewPremise)
            {
                _ValidateNewPremise = validateNewPremise;
                return this;
            }

            public TestContext WithMovingHouseType(MovingHouseType movingHouseType)
            {
                _MovingHouseType = movingHouseType;
                return this;
            }

            public TestContext WithNewMPRN(string newMPRN)
            {
                _NewMPRN = newMPRN;
                return this;
            }

            public TestContext WithNewGPRN(string newGPRN)
            {
                _NewGPRN = newGPRN;
                return this;
            }

            protected override NewPremisePointReferenceNumbersAreValidValidator BuildSut(AutoMocker autoMocker)
            {
                var domainFacade = new DomainFacade();
                domainFacade.SetUpMocker(autoMocker);

                Query = Fixture.Build<MovingHouseValidationQuery>()
                    .With(x => x.ValidateNewPremise, _ValidateNewPremise)
                    .With(x => x.MovingHouseType, _MovingHouseType)
                    .With(x => x.NewMPRN, _NewMPRN)
                    .With(x => x.NewGPRN, _NewGPRN)
                    .Create();

                return base.BuildSut(autoMocker);
            }
        }
    }
}