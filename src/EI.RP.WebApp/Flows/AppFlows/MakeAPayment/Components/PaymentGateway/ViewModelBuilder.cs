using System.Threading.Tasks;


using EI.RP.CoreServices.System;
using System.Linq;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Sap;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using EI.RP.WebApp.Controllers;
using Ei.Rp.Mvc.Core.Controllers;
using EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Steps;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.Banking.PaymentCards;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.DomainServices.Queries.Billing.NextBill;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Infrastructure.Settings;

namespace EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Components.PaymentGateway
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		const decimal LowBalanceLimit = 0.02m;

		private readonly IDomainQueryResolver _domainQueryResolver;
		private readonly IPaymentGatewaySettings _settings;
		private readonly IUserSessionProvider _userSessionProvider;
		private readonly IEncryptionService _encryptionService;
		private readonly ISapSession _sapSession;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IUrlHelper _urlHelper;

		public ViewModelBuilder(
			IDomainQueryResolver queryResolver,
			IPaymentGatewaySettings settings,
			IUserSessionProvider userSessionProvider,
			IEncryptionService encryptionService,
			ISapSession sapSession,
			IHttpContextAccessor contextAccessor,
			IUrlHelper urlHelper)
		{
			_domainQueryResolver = queryResolver;
			_settings = settings;
			_userSessionProvider = userSessionProvider;
			_encryptionService = encryptionService;
			_sapSession = sapSession;
			_contextAccessor = contextAccessor;
			_urlHelper = urlHelper;
		}

		public async Task<ViewModel> Resolve(InputModel componentInput,
			UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			var result = new ViewModel();

			var accountInfo = await _domainQueryResolver.GetAccountInfoByAccountNumber(componentInput.AccountNumber);
			var paymentCards = await _domainQueryResolver.GetPaymentCardsInfo(accountInfo.Partner);

			var businessPartner = accountInfo.Partner;
			var userPaymentCards = paymentCards.ToArray();

			var tasks = Task.WhenAll(SetCurrentBalanceAmount(),
				AddSettingsData(), AddSapSessionData());
			await tasks;

			AddPayerData();
			AddRequestDirectData();
			await AddHashRelatedData();

			return result;

			async Task SetCurrentBalanceAmount()
			{
				var accountBillingInfoByAccountNumber =
					_domainQueryResolver.GetAccountBillingInfoByAccountNumber(componentInput.AccountNumber);
				var nextBillEstimationByAccountNumber =
					_domainQueryResolver.GetNextBillEstimationByAccountNumber(componentInput.AccountNumber);

				var billingInfo = await accountBillingInfoByAccountNumber;

				var estimateYourCostCalculation = await nextBillEstimationByAccountNumber;

				if (componentInput.CurrentBalanceAmount != null)
				{
					result.CurrentBalanceAmount = componentInput.CurrentBalanceAmount;
				}
				else if (estimateYourCostCalculation.CostCanBeEstimated
				         && screenModelContainingTheComponent.GetContainedFlowStartType() ==
				         PaymentFlowInitializer.StartType.FromEstimateCost.ToString())
				{
					result.CurrentBalanceAmount = estimateYourCostCalculation.EstimatedAmount;
				}
				else
				{
					result.CurrentBalanceAmount = billingInfo?.CurrentBalanceAmount;
				}

				result.IsLowBalance = result.CurrentBalanceAmount < LowBalanceLimit;
			}

			async Task AddSapSessionData()
			{
				var csrfEncrypted = _encryptionService.EncryptAsync(_sapSession.SapCsrf);
				var cookieEncrypted = _encryptionService.EncryptAsync(_sapSession.SapJsonCookie);
				result.CSRF =
					(await csrfEncrypted).ToBase64UrlEncoded();

				result.CookieJson =
					(await cookieEncrypted).ToBase64UrlEncoded();
			}

			async Task AddSettingsData()
			{
				var paymentGatewayAccountAsync = _settings.PaymentGatewayAccountAsync();
				var paymentGatewayMerchantIdAsync = _settings.PaymentGatewayMerchantIdAsync();

				result.Account = await paymentGatewayAccountAsync;
				result.MerchantId = await paymentGatewayMerchantIdAsync;
				result.Currency = "EUR";
				result.AutoSettle = _settings.PaymentGatewayAutoSettle;
				result.RequestUrl = _settings.PaymentGatewayRequestUrl;

				var r = _contextAccessor.HttpContext.Request;

				var action = _urlHelper.Action(
					nameof(PaymentGatewayCallbackController.CollectPaymentResult),
					typeof(PaymentGatewayCallbackController).GetNameWithoutSuffix());

				var url = $"{r.Scheme}://{r.Host.Value}{action}";

				result.ResponseUrl = new UriBuilder(url)
				{
					Query = string.Empty
				}.ToString();

				result.ResponseDomain = $"{r.Scheme}://{r.Host.Value}";
			}

			void AddRequestDirectData()
			{
				result.ContractAccountNumber = componentInput.AccountNumber;
				result.BusinessPartner = businessPartner;
				result.ProductId = componentInput.AccountNumber;
				result.Amount = Convert.ToInt32(result.CurrentBalanceAmount.Amount * 100);
			}

			void AddPayerData()
			{
				result.PayerExists = false;
				result.PayerRef = string.Empty;
				result.UserName = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name;

				var cardInfo = userPaymentCards.FirstOrDefault(x =>
					x.IsStandard && x.CardHolder.Equals(result.UserName, StringComparison.InvariantCultureIgnoreCase));

				if (cardInfo != null)
				{
					result.PayerRef = $"{cardInfo.Description}{cardInfo.Issuer}";
					result.PayerExists = true;
				}
			}

			async Task AddHashRelatedData()
			{
				var paymentGatewaySecretAsync = _settings.PaymentGatewaySecretAsync();
				result.TimeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");
				var value = _encryptionService.GetSha1(
					$"{result.TimeStamp}.{result.MerchantId}.{componentInput.AccountNumber}-{result.TimeStamp}.{result.Amount}.{result.Currency}.{result.PayerRef}.");

				result.SHA1HashValue = _encryptionService.GetSha1($"{value}.{await paymentGatewaySecretAsync}");
				result.OrderId = $"{componentInput.AccountNumber}-{result.TimeStamp}";
			}
		}
	}
}