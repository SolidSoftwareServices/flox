using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.MovingHouse.CheckMoveOutRequestResults;
using System.Threading.Tasks;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class IsSapCheckMoveOutOkValidator : IMovingHouseValidator
	{
		private readonly IDomainQueryResolver _queryResolver;

		public IsSapCheckMoveOutOkValidator(
			IDomainQueryResolver queryResolver)
		{
			_queryResolver = queryResolver;
		}
		public string ResolveCacheKey(MovingHouseValidationQuery query)
		{
			return $"{query.ElectricityAccountNumber}-{query.GasAccountNumber}";
		}
		public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
		{
			var electricityIsMoveOutOk =
				string.IsNullOrEmpty(query.ElectricityAccountNumber) ? Task.FromResult(OutputType.Passed) : IsSapCheckMoveOutOk(query.ElectricityAccountNumber);

            var gasIsMoveOutOk =
                string.IsNullOrEmpty(query.GasAccountNumber) ? Task.FromResult(OutputType.Passed) : IsSapCheckMoveOutOk(query.GasAccountNumber);

            return new MovingHouseRulesValidationResult
			{
				Output = (await electricityIsMoveOutOk == OutputType.Passed && await gasIsMoveOutOk == OutputType.Passed) ? OutputType.Passed : OutputType.Failed,
				MovingHouseValidationType = MovingHouseValidationType.IsSapCheckMoveOutOk
			};
		}

		private async Task<OutputType> IsSapCheckMoveOutOk(string accountNumber)
		{
			var accountTask = _queryResolver.GetAccountInfoByAccountNumber(accountNumber);
			var response = await _queryResolver.CheckMoveOut((await accountTask).ContractId);
			if (response.IsMoveOutOk)
			{
				return OutputType.Passed;
			}

			return OutputType.Failed;
		}
	}
}