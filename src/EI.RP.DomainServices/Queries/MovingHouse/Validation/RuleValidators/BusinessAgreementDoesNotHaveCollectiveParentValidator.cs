using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using System.Threading.Tasks;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class BusinessAgreementDoesNotHaveCollectiveParentValidator : IMovingHouseValidator
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly ICompositeRuleValidatorOutputTypeResolver _compositeRuleValidatorOutputResolver;

		public BusinessAgreementDoesNotHaveCollectiveParentValidator(
			IDomainQueryResolver queryResolver,
			ICompositeRuleValidatorOutputTypeResolver compositeRuleValidatorOutputResolver)
		{
			_queryResolver = queryResolver;
			_compositeRuleValidatorOutputResolver = compositeRuleValidatorOutputResolver;
		}

		public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
		{
			MovingHouseValidationType movingHouseValidationType = MovingHouseValidationType.BusinessAgreementDoesNotHaveCollectiveParent;

			var electricityHasCollectiveParentIdTask = 
				string.IsNullOrEmpty(query.ElectricityAccountNumber) ? null : HasCollectiveParentId(query.ElectricityAccountNumber);

			var gasHasCollectiveParentIdTask =
				string.IsNullOrEmpty(query.GasAccountNumber) ? null : HasCollectiveParentId(query.GasAccountNumber);

			var outputTypeTask = _compositeRuleValidatorOutputResolver.Resolve(
				electricityHasCollectiveParentIdTask,
				gasHasCollectiveParentIdTask);

			return new MovingHouseRulesValidationResult
			{
				Output = await outputTypeTask,
				MovingHouseValidationType = movingHouseValidationType
			};
		}

		public string ResolveCacheKey(MovingHouseValidationQuery query)
		{
			return $"{query.ElectricityAccountNumber}-{query.GasAccountNumber}";
		}

		private async Task<OutputType> HasCollectiveParentId(string accountNumber)
		{
			var businessAgreement = (await _queryResolver.GetAccountInfoByAccountNumber(accountNumber)).BusinessAgreement;
			if (string.IsNullOrEmpty(businessAgreement?.CollectiveParentId))
			{
				return OutputType.Passed;
			}

			return OutputType.Failed;
		}
	}
}