using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
    internal class HasAccountDevicesValidator : IMovingHouseValidator
    {
        private readonly IDomainQueryResolver _queryResolver;
        private readonly ICompositeRuleValidatorOutputTypeResolver _compositeRuleValidatorOutputResolver;

        public HasAccountDevicesValidator(IDomainQueryResolver queryResolver, 
                                          ICompositeRuleValidatorOutputTypeResolver compositeRuleValidatorOutputResolver)
        {
            _queryResolver = queryResolver;
            _compositeRuleValidatorOutputResolver = compositeRuleValidatorOutputResolver;
        }
        public string ResolveCacheKey(MovingHouseValidationQuery query)
        {
	        return $"{query.ElectricityAccountNumber}-{query.GasAccountNumber}-{query.ValidateHasAccountDevices}";
        }

        public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
        {
            var movingHouseValidationType = MovingHouseValidationType.HasAccountDevices;

            if (!query.ValidateHasAccountDevices)
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
                Output = await HasDevices(query.ElectricityAccountNumber, query.GasAccountNumber),
                MovingHouseValidationType = movingHouseValidationType
            };
        }

        private async Task<OutputType> HasDevices(string electricityAccountNumber, string gasAccountNumber)
        {
            var electricityTask =
                string.IsNullOrEmpty(electricityAccountNumber) ? null : HasAccountDevices(electricityAccountNumber);

            var gasTask =
                string.IsNullOrEmpty(gasAccountNumber) ? null : HasAccountDevices(gasAccountNumber);

            var outputTypeTask = _compositeRuleValidatorOutputResolver.Resolve(
                electricityTask,
                gasTask);

            return await outputTypeTask;
        }

        private async Task<OutputType> HasAccountDevices(string accountNumber)
        {
            var devices = await _queryResolver.GetDevicesByAccount(accountNumber);
            if (devices != null && devices.Any())
            {
                return OutputType.Passed;
            }

            return OutputType.Failed;
        }
    }
}