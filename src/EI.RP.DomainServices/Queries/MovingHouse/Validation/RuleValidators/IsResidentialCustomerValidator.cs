using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class IsResidentialCustomerValidator : IMovingHouseValidator
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly ICompositeRuleValidatorOutputTypeResolver _compositeRuleValidatorOutputResolver;

		public IsResidentialCustomerValidator(
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
			string.IsNullOrEmpty(query.ElectricityAccountNumber) ? null : IsResidentialCustomer(query.ElectricityAccountNumber);

			var gasTask =
				string.IsNullOrEmpty(query.GasAccountNumber) ? null : IsResidentialCustomer(query.GasAccountNumber);

			var outputTypeTask = _compositeRuleValidatorOutputResolver.Resolve(
				electricityTask,
				gasTask);

			return new MovingHouseRulesValidationResult
			{
				Output = await outputTypeTask,
				MovingHouseValidationType = MovingHouseValidationType.IsResidentialCustomer
			};
		}

		private async Task<OutputType> IsResidentialCustomer(string accountNumber)
		{
			
			var account = (await _queryResolver.GetAccountInfoByAccountNumber(accountNumber));
			if(account.BusinessAgreement.AccountDeterminationID == AccountDeterminationType.ResidentialCustomer)
			{
				return OutputType.Passed;
			}

			return OutputType.Failed;
		}
	}
}