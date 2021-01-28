//using System;
//using EI.RP.DataServices;
//using EI.RP.DomainServices.UnitTests.Infrastructure.DomainMocks;
//using EI.RP.TestServices;
//using Moq.AutoMock;

//namespace EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes
//{
//	abstract class QueryHandlerTestContext<TTargetQueryHandler> : UnitTestContext<TTargetQueryHandler>
//		where TTargetQueryHandler : class
//	{
//		protected override TTargetQueryHandler BuildSut(AutoMocker autoMocker)
//		{
//			_crmUmcClientConfigurator?.Invoke(SapRepositoryMockConfigurator<ISapRepositoryOfCrmUmc>
//				.Configure(autoMocker)

//			);

//			return base.BuildSut(autoMocker);
//		}
//		private Action<SapRepositoryMockConfigurator<ISapRepositoryOfCrmUmc>.ODataClientMockConfigurator> _crmUmcClientConfigurator;
//		public QueryHandlerTestContext<TTargetQueryHandler> WithCrmUmcClient(
//			Action<SapRepositoryMockConfigurator<ISapRepositoryOfCrmUmc>.ODataClientMockConfigurator> crmUmcClientConfigurator)
//		{
//			_crmUmcClientConfigurator = crmUmcClientConfigurator;
//			return this;
//		}
//	}
//}