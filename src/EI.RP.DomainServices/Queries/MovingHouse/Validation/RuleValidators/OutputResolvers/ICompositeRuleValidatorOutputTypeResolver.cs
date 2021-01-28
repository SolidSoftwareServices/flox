using System;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MoveHouse;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers
{
	internal enum CompositeRuleValidatorOutputTypePassIf
	{
		AllPassed=1,
		AnyPassed=2
	}
	internal interface ICompositeRuleValidatorOutputTypeResolver
	{
		Task<OutputType> Resolve(
			Task<OutputType> electricityTask = null,
			Task<OutputType> gasTask = null, CompositeRuleValidatorOutputTypePassIf option=CompositeRuleValidatorOutputTypePassIf.AllPassed);
	}
}