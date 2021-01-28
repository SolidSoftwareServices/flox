using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using Moq.AutoMock;

namespace EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks
{
	static class DomainFacadeExtensions
	{
		public static DomainFacade SetUpMocker(this DomainFacade target, AutoMocker mocker)
		{
			mocker.Use<IDomainQueryResolver>(target.QueryResolver);
			mocker.Use<IDomainCommandDispatcher>(target.CommandDispatcher);
			mocker.Use<IUserSessionProvider>(target.SessionProvider);
			return target;
		}
	}
}