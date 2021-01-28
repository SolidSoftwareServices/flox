using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;

using System.Threading.Tasks;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class IsNotLossInProgressValidator : IMovingHouseValidator
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly ICompositeRuleValidatorOutputTypeResolver _compositeRuleValidatorOutputResolver;

		public IsNotLossInProgressValidator(
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
				string.IsNullOrEmpty(query.ElectricityAccountNumber) ? null : IsNotLossInProgress(query.ElectricityAccountNumber);

			var gasTask =
				string.IsNullOrEmpty(query.GasAccountNumber) ? null : IsNotLossInProgress(query.GasAccountNumber);

			var outputTypeTask = _compositeRuleValidatorOutputResolver.Resolve(
				electricityTask,
				gasTask);

			return new MovingHouseRulesValidationResult
			{
				Output = await outputTypeTask,
				MovingHouseValidationType = MovingHouseValidationType.IsNotLossInProgress
			};
		}

		private async Task<OutputType> IsNotLossInProgress(string accountNumber)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(accountNumber);
			if (account.IsLossInProgress)
			{
				return OutputType.Failed;
			}
			return OutputType.Passed;
		}
	}
}