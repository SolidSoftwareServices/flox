using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;

namespace EI.RP.DomainServices.Queries.MovingHouse.Progress
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<MovingHouseInProgressMovingOutInfo> GetMovingHouseProgressMoveOutInfo(
			this IDomainQueryResolver provider, AccountInfo initiator, AccountInfo other, MovingHouseType moveType,
			bool byPassPipeline = false)
		{
			return await ExecuteProgressQuery<MovingHouseInProgressMovingOutInfo>(provider, initiator, other, moveType,
				byPassPipeline);
		}


		public static async Task<MovingHouseInProgressNewPRNsInfo> GetMovingHouseProgressNewPrnsInfo(
			this IDomainQueryResolver provider, AccountInfo initiator, AccountInfo other, MovingHouseType moveType,
			bool byPassPipeline = false)
		{
			return await ExecuteProgressQuery<MovingHouseInProgressNewPRNsInfo>(provider, initiator, other, moveType,
				byPassPipeline);
		}

		public static async Task<MovingHouseInProgressMovingInInfo> GetMovingHouseProgressMoveInInfo(
			this IDomainQueryResolver provider, AccountInfo initiator, AccountInfo other, MovingHouseType moveType,
			bool byPassPipeline = false)
		{
			return await ExecuteProgressQuery<MovingHouseInProgressMovingInInfo>(provider, initiator, other, moveType,
				byPassPipeline);
		}


		private static async Task<TResult> ExecuteProgressQuery<TResult>(IDomainQueryResolver provider,
			AccountInfo initiator, AccountInfo other, MovingHouseType moveType,
			bool byPassPipeline) where TResult : class, IQueryResult
		{
			var query = new MoveHouseProgressQuery
			{
				MoveType = moveType,
				InitiatedFromAccount = initiator,
				OtherAccount = other
			};
			var result = await provider
				.FetchAsync<MoveHouseProgressQuery, TResult>(query, byPassPipeline);
			return result.SingleOrDefault();
		}
	}
}