using EI.RP.TestServices.Logging;
using NUnit.Framework;
//[assembly:LevelOfParallelism(8)]
namespace EI.RP.WebApp.IntegrationTests
{
	[SetUpFixture]
	public class AssemblySetUp
	{
		

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			
			
		}

		[OneTimeTearDown]
		public void RunAfterAnyTests()
		{
			TestLogging.Default.Flush();
		}
	}
}
