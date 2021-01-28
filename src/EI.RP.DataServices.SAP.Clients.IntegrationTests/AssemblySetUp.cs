using Autofac;
using EI.RP.DataServices.SAP.Clients.IntegrationTests.Infrastructure;
using EI.RP.Stubs.IoC;
using EI.RP.TestServices.Logging;
using NLog;
using NUnit.Framework;

namespace EI.RP.DataServices.SAP.Clients.IntegrationTests
{
	[SetUpFixture]
	public class AssemblySetUp
	{
		public static IContainer Container { get; private set; }


		[OneTimeSetUp]
		public void SetUp()
		{
			TestLogging.Default.ConfigureLogging(minLogLevel:LogLevel.Debug);
			Container = IoCContainerBuilder.From<TestsModule>();
		}

		


		[OneTimeTearDown]
		public void TearDown()
		{
			Container.Dispose();
			Container = null;
		}
	}
}
