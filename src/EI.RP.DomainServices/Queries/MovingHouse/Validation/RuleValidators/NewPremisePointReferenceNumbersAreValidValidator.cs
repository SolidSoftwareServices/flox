using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class NewPremisePointReferenceNumbersAreValidValidator : IMovingHouseValidator
	{
		public string ResolveCacheKey(MovingHouseValidationQuery query)
		{
			return $"{query.ValidateNewPremise}-{query.MovingHouseType}-{query.NewMPRN}-{query.NewGPRN}";
		}

		public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
		{
            var movingHouseValidationType = MovingHouseValidationType.NewPremisePointReferenceNumbersAreValid;

            if (!query.ValidateNewPremise)
            {
                return new MovingHouseRulesValidationResult
                {
                    Output = OutputType.NotExecuted,
                    MovingHouseValidationType = movingHouseValidationType
                };
            }

            if (!query.IsValidQuery(out var nothing))
            {
                return new MovingHouseRulesValidationResult
                {
                    Output = OutputType.Failed,
                    MovingHouseValidationType = movingHouseValidationType
                };
            }            

            if (query.MovingHouseType == null)
            {
                return new MovingHouseRulesValidationResult
                {
                    Output = OutputType.Failed,
                    MovingHouseValidationType = movingHouseValidationType
                };
            }

            var isNewMPRNRequired = query.MovingHouseType.IsOneOf(MovingHouseType.MoveElectricity,
                                                                  MovingHouseType.MoveElectricityAndCloseGas,
                                                                  MovingHouseType.MoveElectricityAndGas,
                                                                  MovingHouseType.MoveGasAndAddElectricity,
                                                                  MovingHouseType.MoveElectricityAndAddGas);

            var isNewGPRNRequired = query.MovingHouseType.IsOneOf(MovingHouseType.MoveElectricityAndGas,
                                                                  MovingHouseType.MoveGas,
                                                                  MovingHouseType.MoveGasAndAddElectricity,
                                                                  MovingHouseType.MoveElectricityAndAddGas);

            if (isNewMPRNRequired && string.IsNullOrEmpty(query.NewMPRN)) {
                return new MovingHouseRulesValidationResult
                {
                    Output = OutputType.Failed,
                    MovingHouseValidationType = movingHouseValidationType
                };
            }

            if (isNewGPRNRequired && string.IsNullOrEmpty(query.NewGPRN))
            {
                return new MovingHouseRulesValidationResult
                {
                    Output = OutputType.Failed,
                    MovingHouseValidationType = movingHouseValidationType
                };
            }

            return new MovingHouseRulesValidationResult
			{
                Output = OutputType.Passed,
				MovingHouseValidationType = movingHouseValidationType
			};
		}

	}
}