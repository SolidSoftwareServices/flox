using System.Threading.Tasks;
using Autofac;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.Stubs.Helpers;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Infrastructure
{
	[TestFixture]

	public abstract class DomainTests
	{


		protected static IDomainQueryResolver DomainQueryProvider { get; } =
			AssemblySetUp.Container.Value.Resolve<IDomainQueryResolver>();


        protected static IDomainCommandDispatcher DomainCommandDispatcher { get; } =
            AssemblySetUp.Container.Value.Resolve<IDomainCommandDispatcher>();

        protected static async Task LoginUser(string userName, string password)
		{
			await AssemblySetUp.Container.Value.Resolve<ISessionOperations>().Login(userName, password);
		}
	}
}