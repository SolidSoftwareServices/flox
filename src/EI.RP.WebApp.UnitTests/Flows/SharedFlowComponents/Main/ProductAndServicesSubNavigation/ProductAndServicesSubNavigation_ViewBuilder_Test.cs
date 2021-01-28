using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.Http.Session;
using Ei.Rp.DomainModels.Competitions;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Queries.Competitions;
using EI.RP.Stubs.CoreServices.Http.Session;
using EI.RP.TestServices;
using EI.RP.WebApp.Flows.SharedFlowComponents.Main.ProductAndServicesSubNavigation;
using EI.RP.WebApp.Infrastructure.Flows;
using EI.RP.WebApp.Infrastructure.Settings;
using EI.RP.WebApp.Infrastructure.StringResources;
using EI.RP.WebApp.UnitTests.Infrastructure;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.SharedFlowComponents.Main.ProductAndServicesSubNavigation
{
    [TestFixture]
    internal class ProductAndServicesSubNavigation_ViewBuilder_Test : UnitTestFixture<ProductAndServicesSubNavigation_ViewBuilder_Test.TestContext, ViewModelBuilder>
    {
        [Theory]
        public async Task Resolve_IsProductsAndServices_Correctly(bool isProductsAndServices)
        {
            var expected = isProductsAndServices;

            var result = await Context
                .WithUserName(Context.DomainFacade.ModelsBuilder.Create<string>())
                .WithIsProductsAndServices(isProductsAndServices)
                .Sut
                .Resolve(Context.BuildInput());

            var actual = result?
                .NavigationItems?
                .FirstOrDefault(x => x.AnchorLink.TestId == "sub-navigation-item-products-and-services")?
                .ClassList
                .Contains("active");

            Assert.AreEqual(expected, actual);
        }

        [Theory]
        public async Task Resolve_IsCompetitions_Correctly(bool isCompetitions)
        {
            var expected = isCompetitions;

            var result = await Context
                .WithUserName(Context.DomainFacade.ModelsBuilder.Create<string>())
                .WithIsCompetitions(isCompetitions)
                .WithIsCompetitionsDismissed(false)
                .WithIsCompetitionsEnabled(true)
                .Sut
                .Resolve(Context.BuildInput());

            var actual = result?
                .NavigationItems?
                .FirstOrDefault(x => x.AnchorLink.TestId == "sub-navigation-item-competitions")?
                .ClassList
                .Contains("active");

            Assert.AreEqual(expected, actual);
        }

        [Theory]
        public async Task Resolve_IsPromotions_Correctly(bool isPromotions)
        {
            var expected = isPromotions;

            var result = await Context
                .WithUserName(Context.DomainFacade.ModelsBuilder.Create<string>())
                .WithIsPromotions(isPromotions)
                .WithIsPromotionsDismissed(false)
                .WithIsPromotionsEnabled(true)
                .Sut
                .Resolve(Context.BuildInput());

            var actual = result?
                .NavigationItems?
                .FirstOrDefault(x => x.AnchorLink.TestId == "sub-navigation-item-promotions")?
                .ClassList
                .Contains("active");

            Assert.AreEqual(expected, actual);
        }

        [Theory]
        public async Task Resolve_NavigationItems_Correctly(bool isCompetitionsEnabled, bool isPromotionsEnabled)
        {
            var expected = 1;

            var result = await Context
                .WithUserName(Context.DomainFacade.ModelsBuilder.Create<string>())
                .WithIsCompetitionsEnabled(isCompetitionsEnabled)
                .WithIsPromotionsEnabled(isPromotionsEnabled)
                .Sut
                .Resolve(Context.BuildInput());

            var navigationItems = result?
                .NavigationItems?
                .ToArray();

            var actual = navigationItems?.Length;

            if (isCompetitionsEnabled)
            {
                expected++;
                Assert.IsNotNull(GetCompetitionsNavigationItem());
            }
            else
            {
                Assert.IsNull(GetCompetitionsNavigationItem());
            }

            if (isPromotionsEnabled)
            {
                expected++;
                Assert.IsNotNull(GetPromotionsNavigationItem());
            }
            else
            {
                Assert.IsNull(GetPromotionsNavigationItem());
            }

            Assert.AreEqual(expected, actual);

            Assert.IsNotNull(navigationItems?
                .FirstOrDefault(x =>
                    x.AnchorLink.Label == "Products & Services" &&
                    x.AnchorLink.TestId == "sub-navigation-item-products-and-services" &&
                    x.AnchorLink.FlowAction.FlowName == "ProductAndServices"));

            NavigationItem GetCompetitionsNavigationItem()
            {
                return navigationItems?
                    .FirstOrDefault(x =>
                        x.AnchorLink.Label == "Competitions" &&
                        x.AnchorLink.TestId == "sub-navigation-item-competitions" &&
                        x.AnchorLink.FlowAction.FlowName == "CompetitionEntry");
            }

            NavigationItem GetPromotionsNavigationItem()
            {
                return navigationItems?
                    .FirstOrDefault(x =>
                        x.AnchorLink.Label == "Promotions" &&
                        x.AnchorLink.TestId == "sub-navigation-item-promotions" &&
                        x.AnchorLink.FlowAction.FlowName == "PromotionEntry");
            }
        }

        public class TestContext : UnitTestContext<ViewModelBuilder>
        {
            private bool _isProductsAndServices;

            private bool _isCompetitions;
            private bool _isCompetitionsEnabled = true;
            private bool _isCompetitionsDismissed;

            private bool _isPromotions;
            private bool _isPromotionsEnabled = true;
            private bool _isPromotionsDismissed;

            private string _userName;

            public DomainFacade DomainFacade { get; } = new DomainFacade();

            public TestContext WithIsProductsAndServices(bool isProductsAndServices)
            {
                _isProductsAndServices = isProductsAndServices;
                return this;
            }

            public TestContext WithIsCompetitions(bool isCompetitions)
            {
                _isCompetitions = isCompetitions;
                return this;
            }

            public TestContext WithIsCompetitionsEnabled(bool isCompetitionsEnabled)
            {
                _isCompetitionsEnabled = isCompetitionsEnabled;
                return this;
            }

            public TestContext WithIsCompetitionsDismissed(bool isCompetitionsDismissed)
            {
                _isCompetitionsDismissed = isCompetitionsDismissed;
                return this;
            }

            public TestContext WithIsPromotions(bool isPromotions)
            {
                _isPromotions = isPromotions;
                return this;
            }

            public TestContext WithIsPromotionsEnabled(bool isPromotionsEnabled)
            {
                _isPromotionsEnabled = isPromotionsEnabled;
                return this;
            }

            public TestContext WithIsPromotionsDismissed(bool isPromotionsDismissed)
            {
                _isPromotionsDismissed = isPromotionsDismissed;
                return this;
            }

            public TestContext WithUserName(string userName)
            {
                _userName = userName;
                return this;
            }

            public InputModel BuildInput()
            {
                return new InputModel
                {
                    IsProductsAndServices = _isProductsAndServices,
                    IsCompetitions = _isCompetitions,
                    IsPromotions = _isPromotions
                };
            }

            protected override ViewModelBuilder BuildSut(AutoMocker autoMocker)
            {
                DomainFacade.SetUpMocker(autoMocker);
                DomainFacade.QueryResolver.Current.Setup(x => x.FetchAsync<CompetitionQuery, CompetitionEntry>(
                        It.IsAny<CompetitionQuery>(),
                        It.IsAny<bool>()))
                    .Returns(Task.FromResult(Enumerable.Empty<CompetitionEntry>()));

                SetupMocks();
                return base.BuildSut(autoMocker);

                void SetupMocks()
                {
                    var cookies = new Mock<IRequestCookieCollection>();
                    var outVal = true.ToString();
                    cookies.Setup(_ => _.TryGetValue(ReusableString.CompetitionnDismissCookieKey, out outVal)).Returns(_isCompetitionsDismissed);
                    cookies.Setup(_ => _.TryGetValue(ReusableString.PromotionDismissCookieKey, out outVal)).Returns(_isPromotionsDismissed);

                    var mockHttpRequest = new Mock<HttpRequest>();
                    mockHttpRequest.Setup(_ => _.Cookies).Returns(cookies.Object);

                    var mockHttpContext = new Mock<HttpContext>();
                    mockHttpContext.Setup(_ => _.Request).Returns(mockHttpRequest.Object);

                    var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
                    mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(mockHttpContext.Object);

                    var uiAppSettings = new Mock<IUiAppSettings>();
                    uiAppSettings.Setup(_ => _.IsCompetitionEnabled).Returns(_isCompetitionsEnabled);
                    uiAppSettings.Setup(_ => _.IsPromotionsEnabled).Returns(_isPromotionsEnabled);

                    autoMocker.Use(uiAppSettings);
                    autoMocker.Use(mockHttpContextAccessor);

                    var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                        new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, _userName),
                            new Claim(ClaimTypes.NameIdentifier, "userId")
                        },
                        "TestAuthType")
                    );
                    var mock = autoMocker.GetMock<FakeSessionProviderStrategy>();
                    mock.SetupGet(c => c.CurrentUserClaimsPrincipal)
                        .Returns(claimsPrincipal);
                    autoMocker.Use<IUserSessionProvider>(mock.Object);
                }
            }
        }
    }
}
