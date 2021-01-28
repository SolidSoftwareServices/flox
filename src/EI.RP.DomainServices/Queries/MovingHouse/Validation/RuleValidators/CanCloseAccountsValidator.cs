using System;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.Contracts.CanCloseAccount;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class CanCloseAccountsValidator : IMovingHouseValidator
	{
		private readonly IDomainQueryResolver _queryResolver;

		public CanCloseAccountsValidator(
			IDomainQueryResolver queryResolver)
		{
			_queryResolver = queryResolver;
		}

		public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
		{
            var movingHouseValidationType = MovingHouseValidationType.CanCloseAccounts;
            if (!query.ValidateCanCloseAccounts)
            {
                return new MovingHouseRulesValidationResult
                {
                    Output = OutputType.NotExecuted,
                    MovingHouseValidationType = movingHouseValidationType
                };
            }

            var canCloseAccountsTask = CanCloseAccounts(query.ElectricityAccountNumber, query.GasAccountNumber, query.MoveOutDate.GetValueOrDefault());

            return new MovingHouseRulesValidationResult
			{
				Output = await canCloseAccountsTask,
				MovingHouseValidationType = movingHouseValidationType
            };
		}
		public string ResolveCacheKey(MovingHouseValidationQuery query)
		{
			return $"{query.ElectricityAccountNumber}-{query.GasAccountNumber}-{query.MoveOutDate.GetValueOrDefault()}";
		}
        private async Task<OutputType> CanCloseAccounts(string electricityAccountNumber, string gasAccountNumber, DateTime closingDate)
        {
            if (closingDate.Equals(DateTime.MinValue)) return OutputType.Failed;

            var canCloseAccountInfo = await _queryResolver.CanCloseAccounts(electricityAccountNumber, gasAccountNumber, closingDate); 
            return canCloseAccountInfo.CanClose ? OutputType.Passed : OutputType.Failed;
        }
    }
}