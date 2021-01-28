using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace EI.RP.CoreServices.Ports.OData
{
	public interface IFluentODataModelQuery<TDto> where TDto:ODataDtoModel
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filterExpression"></param>
		/// <param name="trustSourceFiltering">Whent he odata source does not filter it, applies it in memory</param>
		/// <returns></returns>
		IFluentODataModelQuery<TDto> Filter(Expression<Func<TDto, bool>> filterExpression,bool trustSourceFiltering);
		IFluentODataModelQuery<TDto> OrderBy(Expression<Func<TDto, object>> orderByExpression, bool descending = false);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="expandExpression">
		/// Can expand collections and all collection sub properties e.g.
		/// Installations[0].Devices expands all devices for all installations
		/// </param>
		/// <returns></returns>
		IFluentODataModelQuery<TDto> Expand(Expression<Func<TDto, object>> expandExpression);

		IFluentODataModelQuery<TDto> Key(params object[] keyParts);

		/// <summary>
		/// use when there is only one property in the model of that type
		/// </summary>
		/// <typeparam name="TTargetDto"></typeparam>
		/// <returns></returns>
		IFluentODataModelQuery<TTargetDto> NavigateTo<TTargetDto>() where TTargetDto : ODataDtoModel, new();
		IFluentODataModelQuery<TTargetDto> NavigateTo<TTargetDto>(Expression<Func<TDto, IEnumerable<TTargetDto>>> throughPropertyExpression) where TTargetDto : ODataDtoModel, new();
		IFluentODataModelQuery<TTargetDto> NavigateTo<TTargetDto>(Expression<Func<TDto, TTargetDto>> throughPropertyExpression) where TTargetDto : ODataDtoModel, new();

		Task<IEnumerable<TDto>> GetMany(bool autobatch = true);

		Task<TDto> GetOne();


		// TODO: ADD PAGING
		IFluentODataModelQuery<TDto> Page(int pageIndex,int pageSize);
	}

}