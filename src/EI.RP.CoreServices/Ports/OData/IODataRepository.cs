using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Batching;

namespace EI.RP.CoreServices.Ports.OData
{
	public interface IODataRepository
	{
		Task<string> GetName();
		Task<string> GetEntityContainerName();
		Task<ODataRepositoryMetadata> ResolveMetadata(CancellationToken cancellationToken = default(CancellationToken));
		IBatchAggregator StartNewBatchAggregator();
		IFluentODataModelQuery<TDto> NewQuery<TDto>() where TDto : ODataDtoModel,new();

		Task<TDto> GetOne<TDto>(IFluentODataModelQuery<TDto> query, IBatchAggregator batchAggregator) where TDto : ODataDtoModel,new();
		
		/// <param name="autobatch">set to false when your expect exceptions as part of the normal flow to happen</param>
		Task<TDto> GetOne<TDto>(IFluentODataModelQuery<TDto> query,bool autobatch=true) where TDto : ODataDtoModel,new();
		Task<IEnumerable<TDto>> GetMany<TDto>(IFluentODataModelQuery<TDto> query, IBatchAggregator batchAggregator) where TDto : ODataDtoModel,new();
		
		/// <param name="autobatch">set to false when your expect exceptions as part of the normal flow to happen</param>
		Task<IEnumerable<TDto>> GetMany<TDto>(IFluentODataModelQuery<TDto> query, bool autobatch = true) where TDto : ODataDtoModel,new();
		Task<TResult> ExecuteFunctionWithSingleResult<TResult>(ODataFunction<TResult> function, IBatchAggregator batchAggregator) where TResult : ODataDtoModel, new();
		
		/// <param name="autobatch">set to false when your expect exceptions as part of the normal flow to happen</param>
		Task<TResult> ExecuteFunctionWithSingleResult<TResult>(ODataFunction<TResult> function, bool autobatch = true) where TResult : ODataDtoModel, new();
		Task<IEnumerable<TResult>> ExecuteFunctionWithManyResults<TResult>(ODataFunction<TResult> function, IBatchAggregator batchAggregator) where TResult : ODataDtoModel;
		
		/// <param name="autobatch">set to false when your expect exceptions as part of the normal flow to happen</param>
		Task<IEnumerable<TResult>> ExecuteFunctionWithManyResults<TResult>(ODataFunction<TResult> function, bool autobatch = true) where TResult : ODataDtoModel;


		Task<TDto> AddThenGet<TDto>(TDto newEntity, bool autobatch = true) where TDto : ODataDtoModel;
		Task Add<TDto>(TDto newEntity, bool autobatch = true) where TDto : ODataDtoModel;

        Task Delete<TDto>(TDto existingEntity, bool autobatch = true) where TDto : ODataDtoModel;
        Task<TDto> UpdateThenGet<TDto>(TDto changedEntity, bool autobatch = true) where TDto : ODataDtoModel;
		Task Update<TDto>(TDto changedEntity, bool autobatch = true) where TDto : ODataDtoModel;


	}

}
