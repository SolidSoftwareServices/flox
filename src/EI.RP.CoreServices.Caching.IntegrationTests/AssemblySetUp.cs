using EI.RP.TestServices.Logging;
using NLog;
using NUnit.Framework;

namespace EI.RP.CoreServices.Caching.IntegrationTests
{
	[SetUpFixture]
	public class AssemblySetUp
	{

		[OneTimeSetUp]
		public void SetUp()
		{
			TestLogging.Default.ConfigureLogging(minLogLevel:LogLevel.Trace);
			
		}

	}
}