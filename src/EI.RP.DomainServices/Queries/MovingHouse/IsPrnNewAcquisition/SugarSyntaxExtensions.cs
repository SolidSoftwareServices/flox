using System;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.MovingHouse.IsPrnNewAcquisition
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<bool> IsPrnNewAcquisition(
			this IDomainQueryResolver provider,
			PointReferenceNumber prn,
			bool isPODNewAcquisition,
			bool byPassPipeline = false)
		{
			if (prn == null) throw new ArgumentNullException(nameof(prn));

			var query = new IsPrnNewAcquisitionQuery
			{
				Prn = prn,
				IsPODNewAcquisition = isPODNewAcquisition
			};

			return (await provider.FetchAsync<IsPrnNewAcquisitionQuery, IsPrnNewAcquisitionRequestResult>(query, byPassPipeline))
				.Single()
				.IsNewAcquisition;
		}
	}
}