using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;
using System.Threading.Tasks;
using EI.RP.DomainServices.InternalShared.Accounts;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class HasSameAddressValidator : IMovingHouseValidator
	{
		public IAccountExtraInfoResolver AccountExtraInfoResolver { get; }
		private readonly ICompositeRuleValidatorOutputTypeResolver _compositeRuleValidatorOutputResolver;

		public HasSameAddressValidator(
			ICompositeRuleValidatorOutputTypeResolver compositeRuleValidatorOutputResolver,IAccountExtraInfoResolver accountExtraInfoResolver)
		{
			AccountExtraInfoResolver = accountExtraInfoResolver;
			_compositeRuleValidatorOutputResolver = compositeRuleValidatorOutputResolver;
		}
		public string ResolveCacheKey(MovingHouseValidationQuery query)
		{
			return $"{query.ElectricityAccountNumber}-{query.GasAccountNumber}";
		}
		public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
		{
			var electricityTask =
				string.IsNullOrEmpty(query.ElectricityAccountNumber) ? null : IsBundleMatchForAddress(query.ElectricityAccountNumber);

			var gasTask =
				string.IsNullOrEmpty(query.GasAccountNumber) ? null : IsBundleMatchForAddress(query.GasAccountNumber);

			var outputTypeTask = _compositeRuleValidatorOutputResolver.Resolve(
				electricityTask,
				gasTask);

			return new MovingHouseRulesValidationResult
			{
				Output = await outputTypeTask,
				MovingHouseValidationType = MovingHouseValidationType.HasSameAddress
			};
		}

		private async Task<OutputType> IsBundleMatchForAddress(string accountNumber)
		{
			return await AccountExtraInfoResolver.AddressesMatchForBundle(accountNumber)
				? OutputType.Passed
				: OutputType.Failed;
		}
	}
}