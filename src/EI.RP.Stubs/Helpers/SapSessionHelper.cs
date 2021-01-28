using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DomainServices.Commands.Users.Session.CreateUserSession;

namespace EI.RP.Stubs.Helpers
{
	internal class SapSessionHelper : ISessionOperations
	{
		private readonly IDomainCommandDispatcher _domainCommandDispatcher;

		public SapSessionHelper(IDomainCommandDispatcher domainCommandDispatcher)
		{
			_domainCommandDispatcher = domainCommandDispatcher;
		}

		public async Task Login(string userName, string password)
		{
			await _domainCommandDispatcher.ExecuteAsync(new CreateUserSessionCommand(userName,
				password));
		}
	}
}