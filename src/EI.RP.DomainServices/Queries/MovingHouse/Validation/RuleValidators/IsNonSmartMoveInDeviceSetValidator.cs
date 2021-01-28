using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Metering.Premises;
using NLog;


namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	class IsNonSmartMoveInDeviceSetValidator : IMovingHouseValidator
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IDomainQueryResolver _queryResolver;

		public IsNonSmartMoveInDeviceSetValidator(IDomainQueryResolver domainQueryResolver)
		{
			_queryResolver = domainQueryResolver;
		}
		public string ResolveCacheKey(MovingHouseValidationQuery query)
		{
			return $"{query.NewMPRN}";
		}
		public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
		{
			var outputType = OutputType.NotExecuted;

			if (!string.IsNullOrEmpty(query.NewMPRN))
			{
				var premises = await _queryResolver.GetPremiseByPrn(new ElectricityPointReferenceNumber(query.NewMPRN));
				var devices = premises.Installations.SelectMany(x => x.Devices);
				outputType = devices.Any(x => x.IsSmart) ? OutputType.Failed : OutputType.Passed;
				Logger.Debug($"{nameof(IsNonSmartMoveInDeviceSetValidator)}: {nameof(query.NewMPRN)}:{query.NewMPRN} - {outputType}");
			}

			return new MovingHouseRulesValidationResult
			{
				MovingHouseValidationType = MovingHouseValidationType.IsNonSmartMoveInDeviceSet,
				Output = outputType
			};
		}
	}
}