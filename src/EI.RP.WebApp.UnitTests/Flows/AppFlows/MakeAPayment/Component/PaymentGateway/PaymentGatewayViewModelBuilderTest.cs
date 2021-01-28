using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Queries.Billing.NextBill;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.Stubs.CoreServices.Http.Session;
using EI.RP.TestServices;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Components.PaymentGateway;
using EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Steps;
using EI.RP.WebApp.UnitTests.Infrastructure;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MakeAPayment.Component.PaymentGateway
{
	internal class PaymentGatewayViewModelBuilderTest : UnitTestFixture<PaymentGatewayViewModelBuilderTest.TestContext, ViewModelBuilder>
	{
		public class TestContext : UnitTestContext<ViewModelBuilder>
		{
			private string _accountNumber;

			public DomainFacade DomainFacade { get; } = new DomainFacade();

			public TestContext WithAccountNumber(string accountNumber)
			{
				_accountNumber = accountNumber;
				return this;
			}

			public InputModel BuildInput()
			{
				return new InputModel
				{
					AccountNumber = _accountNumber
				};
			}

			protected override ViewModelBuilder BuildSut(AutoMocker autoMocker)
			{
				DomainFacade.SetUpMocker(autoMocker);

				DomainFacade.QueryResolver.Current.Setup(x =>
						x.FetchAsync<AccountInfoQuery, AccountInfo>(
							It.IsAny<AccountInfoQuery>(), It.IsAny<bool>()))
					.Returns(Task.FromResult(DomainFacade.ModelsBuilder.Create<AccountInfo>().ToOneItemArray()
						.AsEnumerable()));

				DomainFacade.QueryResolver.Current.Setup(x =>
						x.FetchAsync<EstimateNextBillQuery, NextBillEstimation>(
							It.IsAny<EstimateNextBillQuery>(), It.IsAny<bool>()))
					.Returns(Task.FromResult(DomainFacade.ModelsBuilder.Build<NextBillEstimation>()
						.With(x=>x.CostCanBeEstimated, true)
						.Create().ToOneItemArray()
						.AsEnumerable()));
				SetupMocks();
				return base.BuildSut(autoMocker);

				void SetupMocks()
				{
					var httpRequestMock = new Mock<HttpRequest>();
					httpRequestMock.Setup(x => x.Scheme).Returns("https");
					httpRequestMock.Setup(x => x.Host).Returns(new HostString("testHost"));
					var httpContextMock = new Mock<HttpContext>();
					var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

					httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);
					httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

					var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
							new List<Claim>
							{
								new Claim(ClaimTypes.Name, DomainFacade.ModelsBuilder.Create<string>()),
								new Claim(ClaimTypes.NameIdentifier, "userId")
							},
							"TestAuthType")
					);

					var fakeSessionMock = autoMocker.GetMock<FakeSessionProviderStrategy>();
					fakeSessionMock.SetupGet(c => c.CurrentUserClaimsPrincipal)
						.Returns(claimsPrincipal);
					autoMocker.Use<IUserSessionProvider>(fakeSessionMock.Object);
					autoMocker.Use<IHttpContextAccessor>(httpContextAccessorMock.Object);
				}
			}
		}

		[Test]
		public async Task EncryptionIsCalledWithCorrectString()
		{
			var accountNumber = Context.DomainFacade.ModelsBuilder.Create<long>().ToString();
			var containedFlow = new UiFlowScreenModel() { };
			containedFlow.Metadata.ContainedFlowStartType = PaymentFlowInitializer.StartType.FromEstimateCost.ToString();

			var result = await Context
				.WithAccountNumber(accountNumber)
				.Sut
				.Resolve(Context.BuildInput(), containedFlow);

			var expectedToCallGetSha1With =
				$"{result.TimeStamp}.{result.MerchantId}.{accountNumber}-{result.TimeStamp}.{result.Amount}.{result.Currency}.{result.PayerRef}.";

			var encryptionServiceMock = Context.AutoMocker.GetMock<IEncryptionService>();
			encryptionServiceMock.Verify(x => x.GetSha1(expectedToCallGetSha1With), Times.Once);
		}
	}
}