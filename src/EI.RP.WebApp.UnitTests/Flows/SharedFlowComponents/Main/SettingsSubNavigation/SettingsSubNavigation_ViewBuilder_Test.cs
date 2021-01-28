using System.Linq;
using System.Threading.Tasks;
using EI.RP.TestServices;
using EI.RP.WebApp.Flows.SharedFlowComponents.Main.SettingsSubNavigation;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.SharedFlowComponents.Main.SettingsSubNavigation
{
    [TestFixture]
    internal class SettingsSubNavigation_ViewBuilder_Test : UnitTestFixture<SettingsSubNavigation_ViewBuilder_Test.TestContext, ViewModelBuilder>
    {
        [Theory]
        public async Task Resolve_IsMyDetails_Correctly(bool isMyDetails)
        {
            var expected = isMyDetails;

            var result = await Context
                .WithIsMyDetails(isMyDetails)
                .Sut
                .Resolve(Context.BuildInput());

            var actual = result?
                .NavigationItems?
                .FirstOrDefault(x => x.AnchorLink.TestId == "sub-navigation-item-my-details")?
                .ClassList
                .Contains("active");

            Assert.AreEqual(expected, actual);
        }

        [Theory]
        public async Task Resolve_IsChangePassword_Correctly(bool isChangePassword)
        {
	        var expected = isChangePassword;

	        var result = await Context
		        .WithIsChangePassword(isChangePassword)
		        .Sut
		        .Resolve(Context.BuildInput());

	        var actual = result?
		        .NavigationItems?
		        .FirstOrDefault(x => x.AnchorLink.TestId == "sub-navigation-item-change-password")?
		        .ClassList
		        .Contains("active");

	        Assert.AreEqual(expected, actual);
        }

        [Theory]
        public async Task Resolve_IsMarketing_Correctly(bool isMarketing)
        {
            var expected = isMarketing;

            var result = await Context
                .WithIsMarketing(isMarketing)
                .Sut
                .Resolve(Context.BuildInput());

            var actual = result?
                .NavigationItems?
                .FirstOrDefault(x => x.AnchorLink.TestId == "sub-navigation-item-marketing")?
                .ClassList
                .Contains("active");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task Resolve_NavigationItems_Correctly()
        {
            var expected = 3;

            var result = await Context
                .Sut
                .Resolve(Context.BuildInput());

            var navigationItems = result?
                .NavigationItems?
                .ToArray();

            var actual = navigationItems?.Length;

            Assert.AreEqual(expected, actual);

            Assert.IsNotNull(navigationItems?
                .FirstOrDefault(x => 
                    x.AnchorLink.Label == "My Details" &&
                    x.AnchorLink.TestId == "sub-navigation-item-my-details" &&
                    x.AnchorLink.FlowAction.FlowName == "UserContactDetails"));

            Assert.IsNotNull(navigationItems?
	            .FirstOrDefault(x => 
		            x.AnchorLink.Label == "Change Password" &&
		            x.AnchorLink.TestId == "sub-navigation-item-change-password"));

            Assert.IsNotNull(navigationItems?
                .FirstOrDefault(x => 
                    x.AnchorLink.Label == "Marketing" &&
                    x.AnchorLink.TestId == "sub-navigation-item-marketing"));
        }

        public class TestContext : UnitTestContext<ViewModelBuilder>
        {
            private bool _isMyDetails;
            private bool _isChangePassword;
            private bool _isMarketing;

            public TestContext WithIsMyDetails(bool isMyDetails)
            {
	            _isMyDetails = isMyDetails;
                return this;
            }

            public TestContext WithIsChangePassword(bool isChangePassword)
            {
	            _isChangePassword = isChangePassword;
	            return this;
            }

            public TestContext WithIsMarketing(bool isMarketing)
            {
	            _isMarketing = isMarketing;
                return this;
            }

            public InputModel BuildInput()
            {
                return new InputModel
                {
                    IsMyDetails = _isMyDetails,
                    IsChangePassword = _isChangePassword,
                    IsMarketing = _isMarketing
                };
            }
        }
    }
}
