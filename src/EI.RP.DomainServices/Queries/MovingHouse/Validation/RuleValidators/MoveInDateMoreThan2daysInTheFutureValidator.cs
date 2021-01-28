using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class MoveInDateMoreThan2daysInTheFutureValidator : IMovingHouseValidator
	{
		public string ResolveCacheKey(MovingHouseValidationQuery query)
		{
			return $"{query.MoveInDate}";
		}
		public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
		{
            var movingHouseValidationType = MovingHouseValidationType.MoveInDateMoreThan2daysInTheFuture;

            if (!query.ValidateMoveInDetails)
            {
                return new MovingHouseRulesValidationResult
                {
                    Output = OutputType.NotExecuted,
                    MovingHouseValidationType = movingHouseValidationType
                };
            }

            if (!query.IsValidQuery(out var nothing) || query.MoveInDate == null)
            {
                return new MovingHouseRulesValidationResult
                {
                    Output = OutputType.Failed,
                    MovingHouseValidationType = movingHouseValidationType
                };
            }

            return new MovingHouseRulesValidationResult
			{
				Output = query.MoveInDate > DateTime.Today.AddDays(2) ? OutputType.Failed : OutputType.Passed,
				MovingHouseValidationType = movingHouseValidationType
            };
		}

	}
}