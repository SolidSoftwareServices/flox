using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Metering.Premises;
using System.Linq;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class DiscStatusIsNewValidator : IMovingHouseValidator
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly ICompositeRuleValidatorOutputTypeResolver _compositeRuleValidatorOutputResolver;

		public DiscStatusIsNewValidator(
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
				string.IsNullOrEmpty(query.ElectricityAccountNumber) ? null : IsDiscStatusNew(query.ElectricityAccountNumber);

			var gasTask =
				string.IsNullOrEmpty(query.GasAccountNumber) ? null : IsDiscStatusNew(query.GasAccountNumber);

			var outputTypeTask = _compositeRuleValidatorOutputResolver.Resolve(
				electricityTask,
				gasTask);

			return new MovingHouseRulesValidationResult
			{
				Output = await outputTypeTask,
				MovingHouseValidationType = MovingHouseValidationType.DiscStatusIsNew
			};
		}

		private async Task<OutputType> IsDiscStatusNew(string accountNumber)
		{
			var accountInfoTask = _queryResolver.GetAccountInfoByAccountNumber(accountNumber);
			var account = (await accountInfoTask);

			PointReferenceNumber prn = null;
			if(account.IsElectricityAccount())
			{
				prn =  (ElectricityPointReferenceNumber)account.PointReferenceNumber;
			}
			else if (account.IsGasAccount())
			{
				prn = (GasPointReferenceNumber)account.PointReferenceNumber;
			}

			var premiseTask = _queryResolver.GetPremiseByPrn(prn);
			var premise = await premiseTask;
			if (premise.Installations.All(x => x.DiscStatus == InstallationDiscStatusType.New || x.DiscStatus == null))
			{
				return OutputType.Passed;
			}

			return OutputType.Failed;
		}
	}
}