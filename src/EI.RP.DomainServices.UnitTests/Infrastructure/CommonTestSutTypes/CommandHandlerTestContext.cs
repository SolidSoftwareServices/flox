//using System;
//using EI.RP.DataServices;
//using EI.RP.DomainServices.UnitTests.Infrastructure.DomainMocks;
//using EI.RP.TestServices;
//using Moq.AutoMock;

//namespace EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes
//{
//	abstract class CommandHandlerTestContext<TTargetCommandHandler> : UnitTestContext<TTargetCommandHandler>
//		where TTargetCommandHandler : class
//	{
//		protected override TTargetCommandHandler BuildSut(AutoMocker autoMocker)
//		{
//			_crmUmcClientConfigurator?.Invoke(SapRepositoryMockConfigurator<ISapRepositoryOfCrmUmc>.Configure(autoMocker));

//			_crmUmcUrmClientConfigurator?.Invoke(SapRepositoryMockConfigurator<ISapRepositoryOfCrmUmcUrm>.Configure(autoMocker));

//			return base.BuildSut(autoMocker);
//		}
//		private Action<SapRepositoryMockConfigurator<ISapRepositoryOfCrmUmc>.ODataClientMockConfigurator> _crmUmcClientConfigurator;

//		public CommandHandlerTestContext<TTargetCommandHandler> WithCrmUmcClient(
//			Action<SapRepositoryMockConfigurator<ISapRepositoryOfCrmUmc>.ODataClientMockConfigurator> crmUmcClientConfigurator)
//		{
//			_crmUmcClientConfigurator = crmUmcClientConfigurator;
//			return this;
//		}

//		private Action<SapRepositoryMockConfigurator<ISapRepositoryOfCrmUmcUrm>.ODataClientMockConfigurator> _crmUmcUrmClientConfigurator;

//		public CommandHandlerTestContext<TTargetCommandHandler> WithCrmUmcUrmClient(
//			Action<SapRepositoryMockConfigurator<ISapRepositoryOfCrmUmcUrm>.ODataClientMockConfigurator> crmUrmUmcClientConfigurator)
//		{
//			_crmUmcUrmClientConfigurator = crmUrmUmcClientConfigurator;
//			return this;
//		}
//	}
//}