using System.Linq;
using System.Threading.Tasks;
using EI.RP.TestServices;
using EI.RP.WebApp.Flows.AppFlows.Agent.Components.AgentMainNavigation;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Agent.Components.AgentMainNavigation
{
    [TestFixture]
    internal class AgentMainNavigation_ViewBuilder_Test : UnitTestFixture<AgentMainNavigation_ViewBuilder_Test.TestContext, ViewModelBuilder>
    {
        [Test]
        public async Task Resolve_SettingsItems_Correctly()
        {
	        var result = await Context
	            .Sut
                .Resolve(Context.BuildInput());

            var actual = result?
                .SettingsItems?
                .FirstOrDefault(x => x.AnchorLink.TestId == "main-navigation-change-password-link-desktop");

            Assert.IsNotNull(actual);
        }

        public class TestContext : UnitTestContext<ViewModelBuilder>
        {
            public InputModel BuildInput()
            {
                return new InputModel();
            }
        }
    }
}
