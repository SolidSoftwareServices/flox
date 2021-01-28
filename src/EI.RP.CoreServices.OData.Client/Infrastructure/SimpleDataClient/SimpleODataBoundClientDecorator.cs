using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EI.RP.CoreServices.Batching;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using Simple.OData.Client;

namespace EI.RP.CoreServices.OData.Client.Infrastructure.SimpleDataClient
{
	internal class SimpleODataBoundClientDecorator<TDto>:IFluentODataModelQuery<TDto> where TDto:ODataDtoModel,new()
	{


		private readonly IODataRepository _repository;
		public IBoundClient<TDto> BoundClient { get; private set; }

		public SimpleODataBoundClientDecorator(IODataRepository repository, IBoundClient<TDto> boundClient)
		{
			_repository = repository;
			BoundClient = boundClient;
		}

		private readonly List<(Expression<Func<TDto, bool>> expression, bool ensureIsApplied)> _filterExpressionsApplied=new List<(Expression<Func<TDto, bool>> expression, bool ensureIsApplied)>();
		public IEnumerable<(Expression<Func<TDto, bool>> expression,bool ensureIsApplied)> FilterExpressionsApplied => _filterExpressionsApplied;
		public IFluentODataModelQuery<TDto> Filter(Expression<Func<TDto, bool>> filterExpression, bool trustSourceFiltering)
		{
			
			ThrowIfNotFilterable();

			BoundClient=BoundClient.Filter(filterExpression);
			_filterExpressionsApplied.Add((filterExpression,!trustSourceFiltering));
			return this;
			void ThrowIfNotFilterable()
			{
#if DEBUG 
				////unfortunately the API metadata is not reliable as some non-filterable are filterable
				////only needed in debug mode
				//var properties = filterExpression.GetExpressionProperties().Where(x => !x.GetCustomAttributes(typeof(FilterableAttribute), false).Any()).ToArray();
				//if (properties.Any())
				//{
				//	throw new InvalidOperationException($"The attribute(s) are not filterable:{Environment.NewLine} {string.Join(Environment.NewLine,properties.Select(x=>$"{typeof(TDto).FullName}.{x.Name}"))}");
				//}
#endif
			}

		}

		private readonly List<Expression<Func<TDto, object>>> _orderByExpressionsApplied = new List<Expression<Func<TDto, object>>>();
		public IEnumerable<Expression<Func<TDto, object>>> OrderByExpressionsApplied => _orderByExpressionsApplied;
		public IFluentODataModelQuery<TDto> OrderBy(Expression<Func<TDto, object>> orderByExpression,bool descending=false)
		{

			ThrowIfNotSortable();

			BoundClient = descending
				? BoundClient.OrderByDescending(orderByExpression)
				: BoundClient.OrderBy(orderByExpression);

			
			_orderByExpressionsApplied.Add(orderByExpression);
			return this;
			void ThrowIfNotSortable()
			{
#if DEBUG
				//////unfortunately the API metadata is not reliable as some non-filterable are filterable
				////only needed in debug mode
				//var properties = orderByExpression.GetExpressionProperties().Where(x => !x.GetCustomAttributes(typeof(SortableAttribute), false).Any()).ToArray();
				//if (properties.Any())
				//{
				//	throw new InvalidOperationException($"The attribute(s) are not sortable:{Environment.NewLine} {string.Join(Environment.NewLine, properties.Select(x => $"{typeof(TDto).FullName}.{x.Name}"))}");
				//}
#endif
			}

		}

		public IFluentODataModelQuery<TDto> Expand(Expression<Func<TDto, object>> expandExpression)
		{
			var name=expandExpression.GetPropertyNameEx().Replace('.','/');
		
			BoundClient = BoundClient.Expand(name);
			return this;
		}

		public IFluentODataModelQuery<TDto> Key(params object[] keyParts)
		{
			BoundClient.Key(keyParts);
			return this;
		}
		public IFluentODataModelQuery<TTargetDto> NavigateTo<TTargetDto>() where TTargetDto : ODataDtoModel, new()
		{
			return _NavigateTo<TTargetDto>();
		}


		public IFluentODataModelQuery<TTargetDto> NavigateTo<TTargetDto>(Expression<Func<TDto, IEnumerable<TTargetDto>>> throughPropertyExpression) where TTargetDto : ODataDtoModel, new()
		{
			if (throughPropertyExpression == null) throw new ArgumentNullException(nameof(throughPropertyExpression));
			return _NavigateTo<TTargetDto>(throughPropertyExpression.GetPropertyName());
		}

		public IFluentODataModelQuery<TTargetDto> NavigateTo<TTargetDto>(Expression<Func<TDto, TTargetDto>> throughPropertyExpression) where TTargetDto : ODataDtoModel, new()
		{
			if (throughPropertyExpression == null) throw new ArgumentNullException(nameof(throughPropertyExpression));
			return _NavigateTo<TTargetDto>(throughPropertyExpression.GetPropertyName());
		}
		private IFluentODataModelQuery<TTargetDto> _NavigateTo<TTargetDto>(string throughPropertyName = null) where TTargetDto : ODataDtoModel, new()
		{
			var resolveCollectionNameOf = throughPropertyName ?? ODataDtoModel.ResolveCollectionNameOf<TTargetDto>();
			return new SimpleODataBoundClientDecorator<TTargetDto>(_repository, BoundClient.NavigateTo<TTargetDto>(resolveCollectionNameOf));
		}

		
		

		public async Task<IEnumerable<TDto>> GetMany(bool autobatch=true)
		{
			return await _repository.GetMany(this,autobatch: autobatch);
		}

		public async Task<IEnumerable<TDto>> GetMany(IBatchAggregator batchAggregator)
		{
			return await _repository.GetMany(this,batchAggregator);
		}

		public async Task<TDto> GetOne() 
		{
			return await _repository.GetOne((IFluentODataModelQuery<TDto>)this);
		}

		public IFluentODataModelQuery<TDto> Page(int pageIndex, int pageSize)
		{
			throw new NotSupportedException();
		}

		public async Task<TDto> GetOne(IBatchAggregator batchAggregator)
		{
			return await _repository.GetOne((IFluentODataModelQuery<TDto>)this,batchAggregator);
		}
	}
}