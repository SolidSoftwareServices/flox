using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataServices;
using Ei.Rp.DomainModels;
using Ei.Rp.DomainModels.Contracts;
using EI.RP.DomainModels.SpecimenBuilders;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes
{
	abstract class QueryHandlerTest<TTargetQueryHandler, TQuery> : UnitTestFixture<QueryHandlerTest<TTargetQueryHandler,TQuery>.DefaultTestContext, TTargetQueryHandler>
		where TTargetQueryHandler : class, IQueryHandler<TQuery>
		where TQuery : IQueryModel, new()
	{

		public class DefaultTestContext : UnitTestContext<TTargetQueryHandler>
		{
			public DefaultTestContext():base(new Fixture().CustomizeDomainTypeBuilders())
			{
				CrmUmcRepoMock = new Lazy<ODataRepositoryMock<ISapRepositoryOfCrmUmc>>(()=>new ODataRepositoryMock<ISapRepositoryOfCrmUmc>(AutoMocker));
				CrmUmcUrmRepoMock = new Lazy<ODataRepositoryMock<ISapRepositoryOfCrmUmcUrm>>(() => new ODataRepositoryMock<ISapRepositoryOfCrmUmcUrm>(AutoMocker));
				ErpUmcRepoMock = new Lazy<ODataRepositoryMock<ISapRepositoryOfErpUmc>>(() => new ODataRepositoryMock<ISapRepositoryOfErpUmc>(AutoMocker));
				UserManagementRepoMock = new Lazy<ODataRepositoryMock<ISapRepositoryOfUserManagement>>(() => new ODataRepositoryMock<ISapRepositoryOfUserManagement>(AutoMocker));
			}

			public Lazy<ODataRepositoryMock<ISapRepositoryOfUserManagement>> UserManagementRepoMock { get; }

			public Lazy<ODataRepositoryMock<ISapRepositoryOfErpUmc>> ErpUmcRepoMock { get; }

			public Lazy<ODataRepositoryMock<ISapRepositoryOfCrmUmc>> CrmUmcRepoMock { get; }
			public Lazy<ODataRepositoryMock<ISapRepositoryOfCrmUmcUrm>> CrmUmcUrmRepoMock { get; }
		}

	
	}
}
