using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Batching;
using EI.RP.CoreServices.OData.Client.Infrastructure.Validation;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using Microsoft.OData.Edm;
using Simple.OData.Client;

namespace EI.RP.CoreServices.OData.Client
{
	public abstract partial class ODataRepository
	{
	
		public async Task<TResult> ExecuteFunctionWithSingleResult<TResult>(ODataFunction<TResult> function, IBatchAggregator batchAggregator) where TResult : ODataDtoModel, new()
		{
			using (Profiler.Profile(ProfileCategoryId, $"{nameof(ExecuteFunctionWithSingleResult)} of {function.GetType().FullName}"))
			{
				TResult result = default(TResult);
				//await validation together so the batches work
				await  ((IBatchAggregatorEnlister)batchAggregator)
					.Enlist((Func<IODataClient, Task>)(async c =>
					{
						result = await _ExecuteFunctionSingleResult(function,c);
					}));

				return result;
			}
		}

		

		public async Task<TResult> ExecuteFunctionWithSingleResult<TResult>(ODataFunction<TResult> function,bool autobatch=true) where TResult : ODataDtoModel, new()
		{
			var getResult = autobatch
				? ExecuteAutoBatch(batchAggregator => this.ExecuteFunctionWithSingleResult(function, batchAggregator))
				: _ExecuteFunctionSingleResult(function);
			return await getResult;
		}

	

		private static readonly ConcurrentDictionary<string,bool> _repositoryFunctions=new ConcurrentDictionary<string, bool>();

		
		private async Task ValidateRepository<TResult>(ODataFunction<TResult> function) where TResult : ODataDtoModel
		{
			var metadata =(await ResolveMetadata()).EdmModel;
			var val =_repositoryFunctions.GetOrAdd(function.FunctionName,FunctionExistsInRepository);
			if (!val)
			{
				throw new InvalidOperationException(
					$"function: {function.FunctionName} not found on repository {GetType().FullName}");
			}

			bool FunctionExistsInRepository(string name)
			{
				var existsInRepository = metadata .SchemaElements
					.Where(x => x.SchemaElementKind.IsOneOf(EdmSchemaElementKind.Function,EdmSchemaElementKind.Action))
					.Any(x => x.Name == name);
				return existsInRepository;
			}
		}

		public async Task<IEnumerable<TResult>> ExecuteFunctionWithManyResults<TResult>(ODataFunction<TResult> function, bool autobatch = true) where TResult : ODataDtoModel
		{
			return autobatch
				? await ExecuteAutoBatch(batchAggregator => this.ExecuteFunctionWithManyResults(function, batchAggregator))
				: await _ExecuteFunctionWithManyResults(function);
			
			
		}
		public async Task<IEnumerable<TResult>> ExecuteFunctionWithManyResults<TResult>(ODataFunction<TResult> function, IBatchAggregator batchAggregator) where TResult : ODataDtoModel
		{
			using (Profiler.Profile(ProfileCategoryId, $"{nameof(ExecuteFunctionWithManyResults)} of {function.GetType().FullName}"))
			{
				TResult[] result = null;
				//await validation together so the batches work
				await ((IBatchAggregatorEnlister)batchAggregator)
					.Enlist((Func<IODataClient, Task>)(async c => result = await _ExecuteFunctionWithManyResults(function,c)));
				if (result == null)
				{
					//bug in library sap or whatever it should be empty and not null
					return await ExecuteFunctionWithManyResults(function);
				}
				return result;
			}
		}

		private async Task<TResult> _ExecuteFunctionSingleResult<TResult>(ODataFunction<TResult> function,
			IODataClient oDataClient=null) where TResult : ODataDtoModel, new()
		{
			await ThrowIfTypeIsNotDeclaredInContainer(function);
			var validateTask = ValidateRepository(function);
			_modelValidator.Validate(function.QueryAsObject, ProxyModelOperation.Query);
			await validateTask;

			var buildFunction = BuildFunction(function,oDataClient);

			var actual = function.ReturnsComplexType()
				? (await buildFunction.ExecuteAsSingleAsync())?.ToExpandoObject().ToObjectOfType<TResult>()
				: await buildFunction.ExecuteAsSingleAsync<TResult>();



			return actual;
		}

		private async Task<TResult[]> _ExecuteFunctionWithManyResults<TResult>(ODataFunction<TResult> function,
			IODataClient oDataClient=null) where TResult : ODataDtoModel
		{
			await ThrowIfTypeIsNotDeclaredInContainer(function);
			var validateTask = ValidateRepository(function);
			_modelValidator.Validate(function.QueryAsObject, ProxyModelOperation.Query);
			await validateTask;
			return await BuildFunction(function, oDataClient).ExecuteAsArrayAsync<TResult>();
		}


		private IUnboundClient<object> BuildFunction<TResult>(ODataFunction<TResult> odataFunction,IODataClient batchClient)
		{
			//overriden until sap supports this
			batchClient = null;
			var function = (batchClient??Client)
				.Unbound()
				.Function(odataFunction.FunctionName)
				.Set(odataFunction.QueryAsObject);
			foreach (var expand in odataFunction.Expands)
			{
				function = function.Expand(expand.GetPropertyNameEx().Replace('.', '/'));
			}

			
			return function;
		}

		
	}
}