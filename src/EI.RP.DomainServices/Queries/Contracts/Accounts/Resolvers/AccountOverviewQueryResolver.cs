using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Infrastructure.Mappers;
using NLog;

namespace EI.RP.DomainServices.Queries.Contracts.Accounts.Resolvers
{
	internal class AccountOverviewQueryResolver : AccountInfoQueryBaseResolver<AccountOverview>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		public AccountOverviewQueryResolver(IUserSessionProvider userSessionProvider,
			IDomainQueryResolver queryResolver, ISapRepositoryOfCrmUmc crmUmc, IDomainMapper domainMapper,
			IResidentialPortalDataRepository repository, ISapRepositoryOfErpUmc erpRepo) : base(userSessionProvider,
			queryResolver, crmUmc, domainMapper, repository,erpRepo)
		{
		}

	}
}