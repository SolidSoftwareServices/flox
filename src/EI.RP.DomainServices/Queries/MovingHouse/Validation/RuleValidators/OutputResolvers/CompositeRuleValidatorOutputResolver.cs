using Ei.Rp.DomainModels.MoveHouse;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers
{

	internal class CompositeRuleValidatorOutputResolver : ICompositeRuleValidatorOutputTypeResolver
	{
		public async Task<OutputType> Resolve(
			Task<OutputType> electricityTask = null,
			Task<OutputType> gasTask = null,
			CompositeRuleValidatorOutputTypePassIf option = CompositeRuleValidatorOutputTypePassIf.AllPassed)
		{
			var result = OutputType.NotExecuted;
			if (electricityTask != null && gasTask != null)
			{
				var elecResult = await electricityTask;
				var gasResult = await gasTask;
                if (elecResult == OutputType.NotExecuted && gasResult == OutputType.NotExecuted)
                {
                    result = OutputType.NotExecuted;
                }
				else if (option == CompositeRuleValidatorOutputTypePassIf.AllPassed && elecResult.IsOneOf(OutputType.Passed, OutputType.NotExecuted) && gasResult.IsOneOf(OutputType.Passed, OutputType.NotExecuted)
					|| option == CompositeRuleValidatorOutputTypePassIf.AnyPassed && (elecResult == OutputType.Passed || gasResult == OutputType.Passed))
				{
					result = OutputType.Passed;
				}
				else
				{
					result = OutputType.Failed;
				}
			}
			else if (electricityTask != null)
			{
				result = await electricityTask;
			}
			else if (gasTask != null)
			{
				result = await gasTask;
			}

			return result;
		}
	}
}