using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
    internal class HasElectricNewPremiseAccountDetailsInSwitchValidator : IMovingHouseValidator
    {
        private readonly IDomainQueryResolver _queryResolver;

        public HasElectricNewPremiseAccountDetailsInSwitchValidator(
            IDomainQueryResolver queryResolver)
        {
            _queryResolver = queryResolver;
        }
        public string ResolveCacheKey(MovingHouseValidationQuery query)
        {
	        return $"{query.ElectricityAccountNumber}-{query.GasAccountNumber}-{query.ValidateElectricNewPremiseAddressInSwitch}-{query.IsNewAcquisitionElectricity}-{query.IsMPRNAddressInSwitch}";
        }
        public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
        {
            var movingHouseValidationType = MovingHouseValidationType.HasElectricNewPremiseAccountDetailsInSwitch;

            if (!query.ValidateElectricNewPremiseAddressInSwitch)
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

            return new MovingHouseRulesValidationResult
            {
                Output = await HasAddressDetailsInSwitch(query.IsNewAcquisitionElectricity, query.IsMPRNAddressInSwitch),
                MovingHouseValidationType = movingHouseValidationType
            };
        }

        private async Task<OutputType> HasAddressDetailsInSwitch(bool isNewAcquisitionElectricity, bool isMPRNAddressInSwitch)
        {
            if (isNewAcquisitionElectricity && !isMPRNAddressInSwitch) return OutputType.Failed;

            return OutputType.Passed;
        }
    }
}