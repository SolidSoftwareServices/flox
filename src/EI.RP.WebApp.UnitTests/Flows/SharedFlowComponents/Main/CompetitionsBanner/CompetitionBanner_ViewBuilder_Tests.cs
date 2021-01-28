using AutoFixture;
using Ei.Rp.DomainModels.Competitions;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Queries.Competitions;
using EI.RP.Stubs.CoreServices.Http.Session;
using EI.RP.TestServices;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Steps;
using EI.RP.WebApp.Flows.SharedFlowComponents.Main.CompetitionsBanner;
using EI.RP.WebApp.Infrastructure.Settings;
using EI.RP.WebApp.UnitTests.Infrastructure;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.UnitTests.Flows.SharedFlowComponents.Main.CompetitionsBanner
{
	internal class CompetitionBannerViewBuilderTest : UnitTestFixture<CompetitionBannerViewBuilderTest.TestContext, ViewModelBuilder>
	{
		public class TestContext : UnitTestContext<ViewModelBuilder>
		{
			private string _accountNumber;
			private bool _isBannerDisabled;
			private bool _isBannerDismissed;
			private string _userName;
			private ScreenEvent _dismissBannerEvent;
			private ScreenEvent _toCompetitionEvent;

			public DomainFacade DomainFacade { get; } = new DomainFacade();

			public TestContext WithAccountNumber(string accountNumber)
			{
				_accountNumber = accountNumber;
				return this;
			}

			public TestContext WithEvents(ScreenEvent dismissBannerEvent, ScreenEvent toCompetitionEvent)
			{
				_dismissBannerEvent = dismissBannerEvent;
				_toCompetitionEvent = toCompetitionEvent;
				return this;
			}

			public TestContext WithIsBannerDisabled(bool isBannerDisabled)
			{
				_isBannerDisabled = isBannerDisabled;
				return this;
			}

			public TestContext WithIsBannerDismissed(bool isBannerDismissed)
			{
				_isBannerDismissed = isBannerDismissed;
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
					DismissBannerEvent = _dismissBannerEvent,
					ToCompetitionEvent = _toCompetitionEvent,
					AccountNumber = _accountNumber
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
					cookies.Setup(_ => _.TryGetValue(It.IsAny<string>(), out outVal)).Returns(_isBannerDismissed);

					var mockHttpRequest = new Mock<HttpRequest>();
					mockHttpRequest.Setup(_ => _.Cookies).Returns(cookies.Object);

					var mockHttpContext = new Mock<HttpContext>();
					mockHttpContext.Setup(_ => _.Request).Returns(mockHttpRequest.Object);

					var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
					mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(mockHttpContext.Object);

					var uiAppSettings = new Mock<IUiAppSettings>();
					uiAppSettings.Setup(_ => _.IsCompetitionEnabled).Returns(_isBannerDisabled);

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

		[Theory]
		public async Task Resolve_DisabledOrDismissed(bool isBannerDisabled, bool isBannerDismissed)
		{
			var expectedVisible = isBannerDisabled && !isBannerDismissed;
			var accountNumber = Context.DomainFacade.ModelsBuilder.Create<long>().ToString();
			var dismissEvent = AccountSelection.StepEvent.DismissCompetitionNotification;
			var flowEvent = AccountSelection.StepEvent.ToCompetition;

			var result = await Context
				.WithUserName(Context.DomainFacade.ModelsBuilder.Create<string>())
				.WithAccountNumber(accountNumber)
				.WithEvents(dismissEvent, flowEvent)
				.WithIsBannerDisabled(isBannerDisabled)
				.WithIsBannerDismissed(isBannerDismissed)
				.Sut
				.Resolve(Context.BuildInput());

			Assert.IsNotNull(result?.FlowAction);
			Assert.IsNotNull(result?.FlowAction?.TriggerEvent);
			Assert.AreEqual(flowEvent, result?.FlowAction?.TriggerEvent);
			Assert.AreEqual(dismissEvent, result?.DismissBannerFlowAction?.TriggerEvent);
			Assert.AreEqual(1, result?.FlowAction?.EventAdditionalFields?.Count());
			Assert.AreEqual("SelectedAccount.AccountNumber", result?.FlowAction?.EventAdditionalFields?.First().PropertyPath);
			Assert.AreEqual(accountNumber, result?.FlowAction?.EventAdditionalFields?.First().Value);
			Assert.AreEqual(expectedVisible, result?.Visible);
		}

	}
}
