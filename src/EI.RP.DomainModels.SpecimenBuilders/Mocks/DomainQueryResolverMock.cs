using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Moq;

namespace EI.RP.DomainModels.SpecimenBuilders.Mocks
{
	public class DomainQueryResolverMock : IDomainQueryResolver
	{
		public Mock<IDomainQueryResolver> Reset()
		{
			Current = new Mock<IDomainQueryResolver>();
			return Current;
		}

		public Mock<IDomainQueryResolver> Current { get; private set; } = new Mock<IDomainQueryResolver>();

		public Task<IEnumerable<TQueryResult>> FetchAsync<TQueryModel, TQueryResult>(TQueryModel query,bool byPassPipeline=false)
			where TQueryModel : IQueryModel where TQueryResult : class, IQueryResult
		{
			return Current.Object.FetchAsync<TQueryModel, TQueryResult>(query,byPassPipeline);
		}

		public DomainQueryResolverMock ExpectQuery<TQuery, TResult>(TQuery query, IEnumerable<TResult> queryResult)
			where TQuery : IQueryModel where TResult : class, IQueryResult
		{
			Current.Setup(x => x.FetchAsync<TQuery, TResult>(It.Is<TQuery>(y => y.Equals(query)), It.IsAny<bool>()))
				.Returns(Task.FromResult((IEnumerable<TResult>) queryResult.ToArray()));

			return this;
		}
	}
}