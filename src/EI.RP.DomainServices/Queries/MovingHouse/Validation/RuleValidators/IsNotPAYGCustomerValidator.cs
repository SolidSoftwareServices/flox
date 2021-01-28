using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class IsNotPAYGCustomerValidator : IMovingHouseValidator
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly ICompositeRuleValidatorOutputTypeResolver _compositeRuleValidatorOutputResolver;

		public IsNotPAYGCustomerValidator(
			IDomainQueryResolver queryResolver,
			ICompositeRuleValidatorOutputTypeResolver compositeRuleValidatorOutputResolver)
		{
			_queryResolver = queryResolver;
			_compositeRuleValidatorOutputResolver = compositeRuleValidatorOutputResolver;
		}
		public string ResolveCacheKey(MovingHouseValidationQuery query)
		{
			return $"{query.ElectricityAccountNumber}-{query.GasAccountNumber}";
		}
		public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
		{
			var electricityTask =
				string.IsNullOrEmpty(query.ElectricityAccountNumber) ? null : IsNotPAYGCustomer(query.ElectricityAccountNumber);

			var gasTask =
				string.IsNullOrEmpty(query.GasAccountNumber) ? null : IsNotPAYGCustomer(query.GasAccountNumber);

			var outputTypeTask = _compositeRuleValidatorOutputResolver.Resolve(
				electricityTask,
				gasTask);

			return new MovingHouseRulesValidationResult
			{
				Output = await outputTypeTask,
				MovingHouseValidationType = MovingHouseValidationType.IsNotPAYGCustomer
			};
		}

		private async Task<OutputType> IsNotPAYGCustomer(string accountNumber)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(accountNumber);
			if (account.IsPAYGCustomer)
			{
				return OutputType.Failed;
			}
			return OutputType.Passed;
		}
	}
}