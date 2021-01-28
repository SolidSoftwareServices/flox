using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class ContractStartedBeforeTodayValidator : IMovingHouseValidator
	{
		private readonly IDomainQueryResolver _queryResolver;

		public ContractStartedBeforeTodayValidator(IDomainQueryResolver queryResolver)
		{
			_queryResolver = queryResolver;
		}
		public string ResolveCacheKey(MovingHouseValidationQuery query)
		{
			return $"{query.ElectricityAccountNumber}-{query.GasAccountNumber}";
		}
		public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
		{
			bool isValid;
			if (!query.IsValidQuery(out var nothing))
			{
				//validator self-contained
				isValid = false;
			}
			else
			{
				var electricityStartDateToday = AccountContractStartedBeforeToday(query.ElectricityAccountNumber);

				var gasStartDateTask = AccountContractStartedBeforeToday(query.GasAccountNumber);
				isValid = await electricityStartDateToday || await gasStartDateTask;
			}
			
			return new MovingHouseRulesValidationResult
			{
				Output = isValid ? OutputType.Passed : OutputType.Failed,
				MovingHouseValidationType = MovingHouseValidationType.AccountContractStartedBeforeToday
			};
		}

		private async Task<bool> AccountContractStartedBeforeToday(string accountNumber)
		{
			if (string.IsNullOrEmpty(accountNumber)) return false;

			var account = await _queryResolver.GetAccountInfoByAccountNumber(accountNumber,true);
			return account?.ContractStartDate?.Date < DateTime.UtcNow.Date;
		}
	}
}