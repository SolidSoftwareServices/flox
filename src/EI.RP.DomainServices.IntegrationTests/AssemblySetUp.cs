using System;
using Autofac;
using EI.RP.CoreServices.Http.Server;
using EI.RP.CoreServices.Platform;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.Stubs.IoC;
using EI.RP.TestServices.Logging;
using NLog;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests
{
	[SetUpFixture]
	public class AssemblySetUp
	{
		public static Lazy<IContainer> Container { get; private set; }

		public static bool IsInternalDeployment
		{
			get => ((TestSettings)Container.Value.Resolve<IPlatformSettings>()).IsInternalDeployment;
			set
			{
				var preProdSettings = (TestSettings)Container.Value.Resolve<IPlatformSettings>();
				preProdSettings.IsInternalDeployment = value;
			}
		}


		[OneTimeSetUp]
		public void SetUp()
		{
			TestLogging.Default.ConfigureLogging();
			Container =new Lazy<IContainer>(IoCContainerBuilder.From<TestsModule>);
		}

		

		[OneTimeTearDown]
		public void TearDown()
		{
			if (Container.IsValueCreated)
			{
				Container.Value.Dispose();
			}
			Container = null;
			TestLogging.Default.Flush();
		}
	}
}
