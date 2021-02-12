using System.Threading.Tasks;
using NUnit.Framework;
using S3.TestServices;
using S3.UiFlows.Core.Flows;

namespace S3.UiFlows.Core.UnitTests.Flows
{
	[Ignore("TODO")]
	[TestFixture]
	class UiFlow_StartNew_Tests:UnitTestFixture<UiFlow_StartNew_Tests.TestContext, UiFlow>
	{
		public class TestContext : UnitTestContext<UiFlow>
		{

		}
	
		[Test]
		public async Task ItCreatesNewContext()
		{
			Assert.Fail("TODO");
		}

		[Test]
		public async Task WhenInitializationStep_AndNotAuthorized_ReturnsUnauthorizedStep()
		{
			Assert.Fail("TODO");
		}

		[Test]
		public async Task WhenInitializationStep_InvokesInitializeContext()
		{
			Assert.Fail("TODO");
		}

		[Test]
		public async Task WhenInitializationStep_ExecutesInitializationResultTrigger()
		{
			Assert.Fail("TODO");
		}

		[Test]
		public async Task WhenInitializationStep_AndInitializeContextFails_SavesRepositoryWithLastError()
		{
			Assert.Fail("TODO");
		}
		[Test]
		public async Task WhenInitializationStep_AndInitializeContextFails_TriggersErrorOccurred()
		{
			Assert.Fail("TODO");

			//TODO: it triggers the event
			//TODO: it does not validate transtion attempt
			//TODO: it does not invoke on step completing

		}

		[Test]
		public async Task WhenNoInitializationStep_TriggersStart()
		{
			Assert.Fail("TODO");
			//TODO: it triggers the event
			//TODO: it does validates transtion attempt
			//TODO: it does not invoke on step completing
		}
		[Test]
		public async Task WhenEventTriggeringFails_ExecutesErrorOcurred_WithLastErrorInContext()
		{
			Assert.Fail("TODO");
			//TODO: it triggers the event
			//TODO: it does validates transtion attempt
			//TODO: it does not invoke on step completing
		}
		[Test]
		public async Task ItReturnsTheNewStep_AfterSavingTheContext()
		{
			Assert.Fail("TODO");
			//TODO: it triggers the event
			//TODO: it does validates transtion attempt
			//TODO: it does not invoke on step completing
		}



		[Test]
		public async Task ItRestoresStateMachine_OnlyIfNotCreatedYet()
		{
			Assert.Fail("TODO");
		}



	}
}