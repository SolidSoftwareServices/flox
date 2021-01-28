
using System;
using AutoFixture;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DataServices;
using EI.RP.DomainModels.SpecimenBuilders;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;

namespace EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes
{
	abstract class CommandHandlerTest<TTargetCommandHandler, TCommand> : UnitTestFixture<
		CommandHandlerTest<TTargetCommandHandler, TCommand>.DefaultTestContext, TTargetCommandHandler>
		where TTargetCommandHandler : class, ICommandHandler<TCommand>
		where TCommand : IDomainCommand
	{

		public class DefaultTestContext : UnitTestContext<TTargetCommandHandler>
		{
			public DefaultTestContext() : base(new Fixture().CustomizeDomainTypeBuilders())
			{
				CrmUmcRepoMock = new Lazy<ODataRepositoryMock<ISapRepositoryOfCrmUmc>>(() =>
					new ODataRepositoryMock<ISapRepositoryOfCrmUmc>(AutoMocker));
				CrmUmcUrmRepoMock = new Lazy<ODataRepositoryMock<ISapRepositoryOfCrmUmcUrm>>(() =>
					new ODataRepositoryMock<ISapRepositoryOfCrmUmcUrm>(AutoMocker));
				ErpUmcRepoMock = new Lazy<ODataRepositoryMock<ISapRepositoryOfErpUmc>>(() =>
					new ODataRepositoryMock<ISapRepositoryOfErpUmc>(AutoMocker));
				UserManagementRepoMock = new Lazy<ODataRepositoryMock<ISapRepositoryOfUserManagement>>(() =>
					new ODataRepositoryMock<ISapRepositoryOfUserManagement>(AutoMocker));
			}

			public Lazy<ODataRepositoryMock<ISapRepositoryOfUserManagement>> UserManagementRepoMock { get; }

			public Lazy<ODataRepositoryMock<ISapRepositoryOfErpUmc>> ErpUmcRepoMock { get; }

			public Lazy<ODataRepositoryMock<ISapRepositoryOfCrmUmc>> CrmUmcRepoMock { get; }
			public Lazy<ODataRepositoryMock<ISapRepositoryOfCrmUmcUrm>> CrmUmcUrmRepoMock { get; }
		}
	}
}