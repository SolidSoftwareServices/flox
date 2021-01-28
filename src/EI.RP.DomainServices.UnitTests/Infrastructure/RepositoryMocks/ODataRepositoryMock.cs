using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using Moq;
using Moq.AutoMock;

namespace EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks
{
	public class ODataRepositoryMock<TRepository> 
		where TRepository : class, IODataRepository
	{
		private readonly AutoMocker _autoMocker;
		public ODataRepositoryMock(AutoMocker autoMocker)
		{
			_autoMocker = autoMocker;
		}
		public ODataRepositoryMock<TRepository> WithNavigation< TDtoSrc, TDtoDest>()
			where TDtoSrc : ODataDtoModel, new()
			where TDtoDest : ODataDtoModel, new()
		{

			_autoMocker
				.Setup<IFluentODataModelQuery<TDtoSrc>, IFluentODataModelQuery<TDtoSrc>>(x => x.Key(It.IsAny<object[]>()))
				.Returns(_autoMocker.Get<IFluentODataModelQuery<TDtoSrc>>());

			_autoMocker
				.Setup<TRepository, IFluentODataModelQuery<TDtoSrc>>(x => x.NewQuery<TDtoSrc>())
				.Returns(_autoMocker.Get<IFluentODataModelQuery<TDtoSrc>>());


			_autoMocker
				.Setup<IFluentODataModelQuery<TDtoSrc>, IFluentODataModelQuery<TDtoDest>>(x =>
					x.NavigateTo<TDtoDest>())
				.Returns(_autoMocker.Get<IFluentODataModelQuery<TDtoDest>>());

			return this;
		}

		public ODataRepositoryMock<TRepository> MockQuery<TDto>(IEnumerable<TDto> result)
			where TDto : ODataDtoModel, new()
		{
			_autoMocker
				.Setup<IFluentODataModelQuery<TDto>, IFluentODataModelQuery<TDto>>(x =>
					x.Expand(It.IsAny<Expression<Func<TDto, object>>>()))
				.Returns(_autoMocker.Get<IFluentODataModelQuery<TDto>>());
			_autoMocker
				.Setup<IFluentODataModelQuery<TDto>, IFluentODataModelQuery<TDto>>(x =>
					x.Expand(It.IsAny<Expression<Func<TDto,object>>>()))
				.Returns(_autoMocker.Get<IFluentODataModelQuery<TDto>>());

			_autoMocker
				.Setup<IFluentODataModelQuery<TDto>, IFluentODataModelQuery<TDto>>(x =>
					x.Filter(It.IsAny<Expression<Func<TDto, bool>>>(),It.IsAny<bool>()))
				.Returns(_autoMocker.Get<IFluentODataModelQuery<TDto>>());

			_autoMocker
				.Setup<IFluentODataModelQuery<TDto>, IFluentODataModelQuery<TDto>>(x => x.Key(It.IsAny<object[]>()))
				.Returns(_autoMocker.Get<IFluentODataModelQuery<TDto>>());

			_autoMocker
				.Setup<TRepository, IFluentODataModelQuery<TDto>>(x => x.NewQuery<TDto>())
				.Returns(_autoMocker.Get<IFluentODataModelQuery<TDto>>());

			_autoMocker
				.Setup<TRepository, Task<IEnumerable<TDto>>>(x =>
					x.GetMany(It.IsAny<IFluentODataModelQuery<TDto>>(), true))
				.ReturnsAsync(result).Verifiable();
			return this;
		}

		public ODataRepositoryMock<TRepository> MockQuerySingle<TDto>(TDto result)
			where TDto : ODataDtoModel,new()
		{
			MockQuery(result.ToOneItemArray());

			_autoMocker
				.Setup<TRepository, Task<TDto>>(x =>
					x.GetOne(It.IsAny<IFluentODataModelQuery<TDto>>(), true))
				.ReturnsAsync(result).Verifiable();
			return this;
		}
	}
}
