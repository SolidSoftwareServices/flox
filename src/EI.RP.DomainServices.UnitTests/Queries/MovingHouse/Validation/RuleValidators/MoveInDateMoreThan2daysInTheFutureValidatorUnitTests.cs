using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
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
	internal class MoveInDateMoreThan2daysInTheFutureValidatorUnitTests : UnitTestFixture<
        MoveInDateMoreThan2daysInTheFutureValidatorUnitTests.TestContext,
        MoveInDateMoreThan2daysInTheFutureValidator>
	{
        internal class CaseModel
        {
            public bool ValidateMoveInDetails { get; set; }
            public DateTime? MoveInDate { get; set; }
            public OutputType Output { get; set; }

            public override string ToString()
            {
	            return $"{nameof(ValidateMoveInDetails)}: {ValidateMoveInDetails}, {nameof(MoveInDate)}: {MoveInDate}, {nameof(Output)}: {Output}";
            }
        }

        public static IEnumerable<TestCaseData> CanResolveCases()
		{
            var cases = new CaseModel[]
            {
                new CaseModel { ValidateMoveInDetails = false, MoveInDate = null, Output = OutputType.NotExecuted },
                new CaseModel { ValidateMoveInDetails = true, MoveInDate = DateTime.Today.AddDays(-1), Output = OutputType.Passed },
                new CaseModel { ValidateMoveInDetails = true, MoveInDate = DateTime.Today, Output = OutputType.Passed },
                new CaseModel { ValidateMoveInDetails = true, MoveInDate = DateTime.Today.AddDays(1), Output = OutputType.Passed },
                new CaseModel { ValidateMoveInDetails = true, MoveInDate = DateTime.Today.AddDays(2), Output = OutputType.Passed },
                new CaseModel { ValidateMoveInDetails = true, MoveInDate = DateTime.Today.AddDays(3), Output = OutputType.Failed },
            };

            foreach (var caseItem in cases)
            {
                var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
                {
                    Output = caseItem.Output,
                    MovingHouseValidationType =
                            MovingHouseValidationType.MoveInDateMoreThan2daysInTheFuture
                };
                yield return new TestCaseData(caseItem).Returns(movingHouseRulesValidationResult);
            }
        }

        [TestCaseSource(nameof(CanResolveCases))]
        public async Task<MovingHouseRulesValidationResult> CanResolve(CaseModel caseModel)
        {
			return await Context
                .WithValidateMoveInDetails(caseModel.ValidateMoveInDetails)
                .WithMoveIn(caseModel.MoveInDate)
                .Sut
				.Resolve(Context.Query);
		}

		public class TestContext : UnitTestContext<MoveInDateMoreThan2daysInTheFutureValidator>
		{
            private bool _validateMoveInDetails;
            private DateTime? _moveInDate;

			public MovingHouseValidationQuery Query { get; private set; }

            public TestContext WithValidateMoveInDetails(bool validateMoveInDetails)
            {
                _validateMoveInDetails = validateMoveInDetails;
                return this;
            }

            public TestContext WithMoveIn(DateTime? moveInDate)
            {
                _moveInDate = moveInDate;
                return this;
            }

            protected override MoveInDateMoreThan2daysInTheFutureValidator BuildSut(AutoMocker autoMocker)
			{
				var domainFacade = new DomainFacade();
				domainFacade.SetUpMocker(autoMocker);

				Query = Fixture.Build<MovingHouseValidationQuery>()
					.With(x => x.ValidateMoveInDetails, _validateMoveInDetails)
					.With(x => x.MoveInDate, _moveInDate)
                    .Without(x => x.MovingHouseType)
                    .Create();				

				return base.BuildSut(autoMocker);
			}
		}
	}
}