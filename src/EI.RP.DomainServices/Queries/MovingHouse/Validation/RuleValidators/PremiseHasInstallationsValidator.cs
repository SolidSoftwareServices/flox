using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Metering.Premises;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;


namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class PremiseHasInstallationsValidator : IMovingHouseValidator
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly ICompositeRuleValidatorOutputTypeResolver _compositeRuleValidatorOutputResolver;

		public PremiseHasInstallationsValidator(
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
				string.IsNullOrEmpty(query.ElectricityAccountNumber) ? null : HasPremiseInstallation(query.ElectricityAccountNumber);

			var gasTask =
				string.IsNullOrEmpty(query.GasAccountNumber) ? null : HasPremiseInstallation(query.GasAccountNumber);

			var outputTypeTask = _compositeRuleValidatorOutputResolver.Resolve(
				electricityTask,
				gasTask);

			return new MovingHouseRulesValidationResult
			{
				Output = await outputTypeTask,
				MovingHouseValidationType = MovingHouseValidationType.HasPremiseInstallations
			};
		}

		private async Task<OutputType> HasPremiseInstallation(string accountNumber)
		{
			var accountInfoTask = _queryResolver.GetAccountInfoByAccountNumber(accountNumber);
			var account = await accountInfoTask;
			PointReferenceNumber prn = null;
			if (account.IsElectricityAccount())
			{
				prn = (ElectricityPointReferenceNumber)account.PointReferenceNumber;
			}
			else if (account.IsGasAccount())
			{
				prn = (GasPointReferenceNumber)account.PointReferenceNumber;
			}

			var premiseTask = _queryResolver.GetPremiseByPrn(prn);
			var premise = await premiseTask;
			if(premise != null && premise.Installations != null)
			{
				return OutputType.Passed;
			}

			return OutputType.Failed;
		}
	}
}