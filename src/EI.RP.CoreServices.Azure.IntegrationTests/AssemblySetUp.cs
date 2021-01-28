using Autofac;
using EI.RP.CoreServices.Azure.Infrastructure.IoC;
using EI.RP.Stubs.IoC;
using EI.RP.TestServices.Logging;
using NLog;
using NUnit.Framework;

namespace EI.RP.CoreServices.Azure.IntegrationTests
{
	[SetUpFixture]
	public class AssemblySetUp
	{
		public static IContainer Container { get; private set; }

		[OneTimeSetUp]
		public void SetUp()
		{
			TestLogging.Default.ConfigureLogging(minLogLevel:LogLevel.Trace);
			Container = IoCContainerBuilder.From<AzureModule>();
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			Container.Dispose();
			Container = null;
		}
	}
}
