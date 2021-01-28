using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Metering.Devices;


namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	class IsNonSmartMoveOutDeviceSetValidator  : IMovingHouseValidator
	{
		private readonly IDomainQueryResolver _queryResolver;

		public IsNonSmartMoveOutDeviceSetValidator(IDomainQueryResolver domainQueryResolver)
		{
			_queryResolver = domainQueryResolver;
		}
		public string ResolveCacheKey(MovingHouseValidationQuery query)
		{
			return $"{query.ElectricityAccountNumber}";
		}
		public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
		{
			var outputType = OutputType.NotExecuted;

			if (!string.IsNullOrEmpty(query.ElectricityAccountNumber))
			{
				var devices = await _queryResolver.GetDevicesByAccount(query.ElectricityAccountNumber);
				outputType = devices.Any(x => x.IsSmart) ? OutputType.Failed : OutputType.Passed;
			}

			return new MovingHouseRulesValidationResult
			{
				MovingHouseValidationType = MovingHouseValidationType.IsNonSmartMoveOutDeviceSet,
				Output = outputType
			};
		}
	}
}