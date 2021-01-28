using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.TestServices;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Steps;
using EI.RP.WebApp.Flows.SharedFlowComponents.Main.PromotionsBanner;
using EI.RP.WebApp.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.UnitTests.Flows.SharedFlowComponents.Main.PromotionsBanner
{
	[TestFixture]
	internal class PromotionsBanner_ViewBuilder_Test : UnitTestFixture<PromotionsBanner_ViewBuilder_Test.TestContext, ViewModelBuilder>
	{
		public class TestContext : UnitTestContext<ViewModelBuilder>
		{
			private string _accountNumber;
			private bool _isBannerDisabled;
			private bool _isBannerDismissed;
			private ScreenEvent _toPromotionEvent;
			private ScreenEvent _dismissBannerEvent;

			public DomainFacade DomainFacade { get; } = new DomainFacade();

			public TestContext WithAccountNumber(string accountNumber)
			{
				_accountNumber = accountNumber;
				return this;
			}

			public TestContext WithEvents(ScreenEvent dismissBannerEvent, ScreenEvent toPromotionEvent)
			{
				_dismissBannerEvent = dismissBannerEvent;
				_toPromotionEvent = toPromotionEvent;
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

			public InputModel BuildInput()
			{
				return new InputModel
				{
					DismissBannerEvent = _dismissBannerEvent,
					ToPromotionEvent = _toPromotionEvent,
					AccountNumber = _accountNumber
				};
			}

			protected override ViewModelBuilder BuildSut(AutoMocker autoMocker)
			{
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
					uiAppSettings.Setup(_ => _.IsPromotionsEnabled).Returns(_isBannerDisabled);

					autoMocker.Use(uiAppSettings);
					autoMocker.Use(mockHttpContextAccessor);
				}
			}
		}

		[Theory]
		public async Task Resolve_DisabledOrDismissed(bool isBannerDisabled, bool isBannerDismissed)
		{
			var expectedVisible = isBannerDisabled && !isBannerDismissed;
			var accountNumber = Context.DomainFacade.ModelsBuilder.Create<long>().ToString();
			var dismissEvent = AccountSelection.StepEvent.DismissPromotionsNotification;
			var flowEvent = AccountSelection.StepEvent.ToPromotion;

			var result = await Context
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
			Assert.AreEqual(result?.FlowAction?.EventAdditionalFields?.Count(), 1);
			Assert.AreEqual(result?.FlowAction?.EventAdditionalFields?.First().PropertyPath, "SelectedAccount.AccountNumber");
			Assert.AreEqual(result?.FlowAction?.EventAdditionalFields?.First().Value, accountNumber);
			Assert.AreEqual(expectedVisible, result?.Visible);
		}
	}
}
