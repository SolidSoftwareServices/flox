using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.Metering.Premises;

namespace EI.RP.DomainServices.Queries.MovingHouse.IsPrnNewAcquisition
{
	internal class IsPrnNewAcquisitionQueryHandler : QueryHandler<IsPrnNewAcquisitionQuery>
	{
        private readonly IDomainQueryResolver _queryResolver;

        public IsPrnNewAcquisitionQueryHandler(IDomainQueryResolver queryResolver)
		{
            _queryResolver = queryResolver;
		}

		protected override Type[] ValidQueryResultTypes { get; } = { typeof(IsPrnNewAcquisitionRequestResult) };

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			IsPrnNewAcquisitionQuery queryModel)
		{
			var isNewAcquisition = await IsNewAcquisition(queryModel.Prn, queryModel.IsPODNewAcquisition);
			var requestResult = new IsPrnNewAcquisitionRequestResult()
			{
				IsNewAcquisition = isNewAcquisition
			};

			return new[]
			{
                requestResult
            }.Cast<TQueryResult>().ToArray();
		}

		private async Task<bool> IsNewAcquisition(PointReferenceNumber prn,
										  bool isNewPointsOfDeliveryNewAcquisition)
		{
			if (isNewPointsOfDeliveryNewAcquisition) return true;
			var isNewPrnInstallationDeregistered = await IsNewPrnInstallationDeregistered(prn);
			if (isNewPrnInstallationDeregistered) return true;

			return false;
		}

		private async Task<bool> IsNewPrnInstallationDeregistered(PointReferenceNumber prn)
		{
			var premise = await _queryResolver.GetPremiseByPrn(prn);
			if (premise?.Installations == null || !premise.Installations.Any())
				return true;

			if (premise.Installations.All(x => x.DeregStatus == DeregStatusType.Deregistered))
				return true;

			return false;
		}
	}
}