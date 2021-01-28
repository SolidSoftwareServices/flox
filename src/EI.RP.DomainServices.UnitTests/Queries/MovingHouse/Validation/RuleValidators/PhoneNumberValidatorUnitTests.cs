using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using Ei.Rp.DomainModels.User;
using EI.RP.CoreServices.System;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators;
using EI.RP.DomainServices.Queries.User.PhoneMetaData;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.Validation.RuleValidators
{
    [TestFixture]
    public class PhoneNumberValidatorUnitTests : UnitTestFixture<
        PhoneNumberValidatorUnitTests.TestContext,
        PhoneNumberValidator>
    {
        public class CaseModel
        {
            public bool ValidatePhoneNumber { get; set; }
            public string PhoneNumber { get; set; }
            public MovingHouseType MovingHouseType { get; set; }
            public OutputType Output { get; set; }
            public PhoneMetadataType PhoneMetadataType { get; set; }

            public override string ToString()
            {
	            return $"{nameof(ValidatePhoneNumber)}: {ValidatePhoneNumber}, {nameof(PhoneNumber)}: {PhoneNumber??"null"}, {nameof(MovingHouseType)}: {MovingHouseType}, {nameof(Output)}: {Output}, {nameof(PhoneMetadataType)}: {PhoneMetadataType}";
            }
        }

        public static IEnumerable<TestCaseData> CanResolveCases()
        {
            var emptyPhoneNumber = "";
            var invalidPhoneNumber = "+3538991235";
            var invalidPhoneNumberWithOutCountryCode = "08998";
            var testPhoneNumber = "0219105044";

            var cases = new CaseModel[]
            {
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricity, PhoneNumber = invalidPhoneNumberWithOutCountryCode, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricity, PhoneNumber = null, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid},
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricity, PhoneNumber = emptyPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricity, PhoneNumber = invalidPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = false, MovingHouseType = MovingHouseType.MoveElectricity, PhoneNumber = null, Output = OutputType.NotExecuted, PhoneMetadataType = PhoneMetadataType.Invalid },

                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricityAndCloseGas, PhoneNumber = testPhoneNumber, Output = OutputType.Passed, PhoneMetadataType = PhoneMetadataType.LandLine },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricityAndCloseGas, PhoneNumber = invalidPhoneNumberWithOutCountryCode, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricityAndCloseGas, PhoneNumber = null, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricityAndCloseGas, PhoneNumber = emptyPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricityAndCloseGas, PhoneNumber = invalidPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = false, MovingHouseType = MovingHouseType.MoveElectricityAndCloseGas, PhoneNumber = null, Output = OutputType.NotExecuted, PhoneMetadataType = PhoneMetadataType.Invalid },

                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveGas, PhoneNumber = invalidPhoneNumberWithOutCountryCode, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveGas, PhoneNumber = null, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveGas, PhoneNumber = emptyPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveGas, PhoneNumber = invalidPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = false, MovingHouseType = MovingHouseType.MoveGas, PhoneNumber = null, Output = OutputType.NotExecuted, PhoneMetadataType = PhoneMetadataType.Invalid },

                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveGasAndAddElectricity, PhoneNumber = invalidPhoneNumberWithOutCountryCode, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveGasAndAddElectricity, PhoneNumber = null, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveGasAndAddElectricity, PhoneNumber = emptyPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveGasAndAddElectricity, PhoneNumber = invalidPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = false, MovingHouseType = MovingHouseType.MoveGasAndAddElectricity, PhoneNumber = null, Output = OutputType.NotExecuted, PhoneMetadataType = PhoneMetadataType.Invalid },

                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricityAndAddGas, PhoneNumber = invalidPhoneNumberWithOutCountryCode, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricityAndAddGas, PhoneNumber = null, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricityAndAddGas, PhoneNumber = emptyPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricityAndAddGas, PhoneNumber = invalidPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = false, MovingHouseType = MovingHouseType.MoveElectricityAndAddGas, PhoneNumber = null, Output = OutputType.NotExecuted, PhoneMetadataType = PhoneMetadataType.Invalid },

                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricityAndGas, PhoneNumber = invalidPhoneNumberWithOutCountryCode, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricityAndGas, PhoneNumber = null, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricityAndGas, PhoneNumber = emptyPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.MoveElectricityAndGas, PhoneNumber = invalidPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = false, MovingHouseType = MovingHouseType.MoveElectricityAndGas, PhoneNumber = null, Output = OutputType.NotExecuted, PhoneMetadataType = PhoneMetadataType.Invalid },

                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.CloseElectricity, PhoneNumber = invalidPhoneNumberWithOutCountryCode, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.CloseElectricity, PhoneNumber = null, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.CloseElectricity, PhoneNumber = emptyPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.CloseElectricity, PhoneNumber = invalidPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = false, MovingHouseType = MovingHouseType.CloseElectricity, PhoneNumber = null, Output = OutputType.NotExecuted, PhoneMetadataType = PhoneMetadataType.Invalid },

                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.CloseGas, PhoneNumber = invalidPhoneNumberWithOutCountryCode, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.CloseGas, PhoneNumber = null, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.CloseGas, PhoneNumber = emptyPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.CloseGas, PhoneNumber = invalidPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = false, MovingHouseType = MovingHouseType.CloseGas, PhoneNumber = null, Output = OutputType.NotExecuted, PhoneMetadataType = PhoneMetadataType.Invalid },

                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.CloseElectricityAndGas, PhoneNumber = invalidPhoneNumberWithOutCountryCode, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.CloseElectricityAndGas, PhoneNumber = null, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.CloseElectricityAndGas, PhoneNumber = emptyPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = true, MovingHouseType = MovingHouseType.CloseElectricityAndGas, PhoneNumber = invalidPhoneNumber, Output = OutputType.Failed, PhoneMetadataType = PhoneMetadataType.Invalid },
                new CaseModel { ValidatePhoneNumber = false, MovingHouseType = MovingHouseType.CloseElectricityAndGas, PhoneNumber = null, Output = OutputType.NotExecuted, PhoneMetadataType = PhoneMetadataType.Invalid },
            };

            foreach (var caseItem in cases)
            {
                var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
                {
                    Output = caseItem.Output,
                    MovingHouseValidationType = MovingHouseValidationType.PhoneNumberIsValid
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
                .WithValidatePhoneNumber(caseModel.ValidatePhoneNumber)
                .WithMovingHouseType(caseModel.MovingHouseType)
                .WithPhoneNumber(caseModel.PhoneNumber)
                .WithPhoneMetadataType(caseModel.PhoneMetadataType)
                .Sut
                .Resolve(Context.Query);
        }

        public class TestContext : UnitTestContext<PhoneNumberValidator>
        {
            private bool _validatePhoneNumber;
            private string _phoneNumber;
            private MovingHouseType _movingHouseType;
            private PhoneMetadataType _phoneMetadataType;

            public MovingHouseValidationQuery Query { get; private set; }

            public TestContext WithValidatePhoneNumber(bool validatePhoneNumber)
            {
                _validatePhoneNumber = validatePhoneNumber;
                return this;
            }

            public TestContext WithPhoneNumber(string phoneNumber)
            {
                _phoneNumber = phoneNumber;
                return this;
            }

            public TestContext WithMovingHouseType(MovingHouseType movingHouseType)
            {
                _movingHouseType = movingHouseType;
                return this;
            }

            public TestContext WithPhoneMetadataType(PhoneMetadataType phoneMetadataType)
            {
                _phoneMetadataType = phoneMetadataType;
                return this;
            }

            protected override PhoneNumberValidator BuildSut(AutoMocker autoMocker)
            {
                var domainFacade = new DomainFacade();
                domainFacade.SetUpMocker(autoMocker);

                var query = new PhoneMetadataResolverQuery
                {
                    PhoneNumber = _phoneNumber
                };

                var result = new PhoneMetaData
                {
                    ContactNumberType = _phoneMetadataType
                };

                domainFacade.QueryResolver.ExpectQuery<PhoneMetadataResolverQuery, PhoneMetaData>
                    (query, result.ToOneItemArray());

                Query = Fixture.Build<MovingHouseValidationQuery>()
                    .With(x => x.ValidatePhoneNumber, _validatePhoneNumber)
                    .With(x => x.MovingHouseType, _movingHouseType)
                    .With(x => x.PhoneNumber, _phoneNumber)
                    .Create();

                return base.BuildSut(autoMocker);
            }
        }

    }
}
