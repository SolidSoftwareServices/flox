using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers
{
    [TestFixture]
    internal class CompositeRuleValidatorOutputResolverTests
    {

        public static IEnumerable<TestCaseData> CanResolveCases()
        {
            var outputTypes = Enum.GetValues(typeof(OutputType)).Cast<OutputType?>().ToList();
            outputTypes.Insert(0, null);

            foreach (var electricityOutputType in outputTypes)
            {
                foreach (var gasOutputType in outputTypes)
                {
                    foreach (var @operator in Enum.GetValues(typeof(CompositeRuleValidatorOutputTypePassIf)).Cast<CompositeRuleValidatorOutputTypePassIf>())
                    {
                        var result = ResolveExpected(electricityOutputType, gasOutputType, @operator);
                        yield return new TestCaseData(
                                electricityOutputType == null ? null : Task.Run(() => electricityOutputType.Value),
                                gasOutputType == null ? null : Task.Run(() => gasOutputType.Value),
                                @operator
                            )
                            .SetName($"{electricityOutputType} - {gasOutputType} -{@operator} ==>{result}")
                            .Returns(result);
                    }
                }
            }
            OutputType ResolveExpected(OutputType? electricityOutputType, OutputType? gasOutputType, CompositeRuleValidatorOutputTypePassIf @operator)
            {
                var expected = OutputType.NotExecuted;
                if (electricityOutputType.HasValue && gasOutputType.HasValue)
                {
                    var types = new[] { electricityOutputType.Value, gasOutputType.Value };
                    switch (@operator)
                    {
                        case CompositeRuleValidatorOutputTypePassIf.AllPassed:
                            expected = types.All(x => x ==OutputType.NotExecuted) ? OutputType.NotExecuted :
                                types.All(
                                    x => x.IsOneOf(OutputType.Passed, OutputType.NotExecuted))
                                    ? OutputType.Passed
                                    : OutputType.Failed;
                            break;
                        case CompositeRuleValidatorOutputTypePassIf.AnyPassed:
                            expected = types.All(x => x == OutputType.NotExecuted) ? OutputType.NotExecuted :
                                types.Any(x => x == OutputType.Passed)
                                    ? OutputType.Passed
                                    : OutputType.Failed;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null);
                    }
                }
                else if (electricityOutputType.HasValue)
                {
                    expected = electricityOutputType.Value;
                }
                else if (gasOutputType.HasValue)
                {
                    expected = gasOutputType.Value;
                }

                return expected;
            }
        }



        [TestCaseSource(nameof(CanResolveCases))]
        public async Task<OutputType> CanResolve(Task<OutputType> electricityTask, Task<OutputType> gasTask, CompositeRuleValidatorOutputTypePassIf @operator)
        {
            var sut = new CompositeRuleValidatorOutputResolver();

            return await sut.Resolve(electricityTask, gasTask, @operator);
        }
    }
}