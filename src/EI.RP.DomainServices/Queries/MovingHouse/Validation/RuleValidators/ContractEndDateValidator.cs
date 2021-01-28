using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;


namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class ContractEndDateIsValidValidator : IMovingHouseValidator
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly ICompositeRuleValidatorOutputTypeResolver _compositeRuleValidatorOutputResolver;

		public ContractEndDateIsValidValidator(
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
			var electricityTask = string.IsNullOrEmpty(query.ElectricityAccountNumber) ?
					null :
					ContractEndDateIsValid(query.ElectricityAccountNumber);

			var gasTask = string.IsNullOrEmpty(query.GasAccountNumber) ?
					 null :
					ContractEndDateIsValid(query.GasAccountNumber);

			var outputType = _compositeRuleValidatorOutputResolver.Resolve(electricityTask, gasTask);

			return new MovingHouseRulesValidationResult
			{
				Output = await outputType,
				MovingHouseValidationType = MovingHouseValidationType.ContractEndDateIsValid
			};
		}

		private async Task<OutputType> ContractEndDateIsValid(string accountNumber)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(accountNumber);
			if(account.ContractEndDate.Value.Date == new System.DateTime(9999, 12, 31).Date)
			{
				return OutputType.Passed;
			}

			return OutputType.Failed;
		}
	}
}