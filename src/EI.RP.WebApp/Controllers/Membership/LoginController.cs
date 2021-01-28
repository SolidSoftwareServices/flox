using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Platform;
using Ei.Rp.DomainErrors;
using EI.RP.DomainServices.Commands.Users.Session.CreateUserSession;
using EI.RP.DomainServices.Infrastructure.Settings;
using EI.RP.DomainServices.ModelExtensions;
using Ei.Rp.Mvc.Core.Controllers;
using Ei.Rp.Mvc.Core.Cryptography.Urls;
using Ei.Rp.Mvc.Core.System;
using EI.RP.UiFlows.Mvc.Controllers;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.Accounts.FlowDefinitions;
using EI.RP.WebApp.Models.Membership;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ControllerBase = Ei.Rp.Mvc.Core.Controllers.ControllerBase;

namespace EI.RP.WebApp.Controllers.Membership
{

	public class LoginController : ControllerBase
	{
		private readonly IDomainCommandDispatcher _domainCommandDispatcher;
		private readonly IUserSessionProvider _userSessionProvider;
		private readonly IEncryptionService _encryptionService;
		private readonly IDomainSettings _domainSettings;

		public LoginController(IDomainCommandDispatcher domainCommandDispatcher, IUserSessionProvider userSessionProvider, IEncryptionService encryptionService, IDomainSettings domainSettings)
		{
			_domainCommandDispatcher = domainCommandDispatcher;
			_userSessionProvider = userSessionProvider;
			_encryptionService = encryptionService;
			_domainSettings = domainSettings;
		}

		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> Login(string returnUrl, LoginViewModel viewModel)
		{
			return await this.HandleActionAsync(async () =>
			{
				if (returnUrl != null && returnUrl.EndsWith($"/{GetType().GetNameWithoutSuffix()}/{nameof(Logout)}"))
				{
					returnUrl = null;
					await _userSessionProvider.EndCurrentSession();
				}

				viewModel.ReturnUrl = returnUrl;
				viewModel.Source = ResolveSource();

				return _userSessionProvider.IsAnonymous()
					? View("LoginFormView", viewModel)
					: await ResolveRedirectionResult(returnUrl, viewModel.Source);
			});

			AppLoginType ResolveSource()
			{
				var result = AppLoginType.Default;

				if (_domainSettings.IsInternalDeployment)
				{
					result = AppLoginType.InternalDeploymentLogin;
				}
				else
				{
					if (!string.IsNullOrWhiteSpace(viewModel.Source))
					{
						result = AppLoginType.From(viewModel.Source);
					}
					else if (Request.Query != null &&
							 (Request.Query.ContainsKey("meterreading") || Request.Query.ContainsKey("meterread")))
					{
						result = AppLoginType.MeterReading;
					}
					else if (Request.Query != null && Request.Query.ContainsKey("addgas"))
					{
						result = AppLoginType.AddGas;
					}
					else if (Request.Query != null &&
							 (Request.Query.ContainsKey("Marketing") ||
							 Request.Query.ContainsKey("marketing")))
					{
						result = AppLoginType.MarketingPreferences;
					}
				}

				return result;
			}
		}

		private async Task<IActionResult> ResolveRedirectionResult(string returnUrl, string source)
		{
			if (string.IsNullOrEmpty(returnUrl))
			{
				var flowName = !_domainSettings.IsInternalDeployment ? nameof(ResidentialPortalFlowType.Accounts) : nameof(ResidentialPortalFlowType.Agent);

				if (string.IsNullOrEmpty(source) || source == AppLoginType.Default.ToString())
				{
					returnUrl = $"~/{flowName}/{nameof(IUiFlowController.Init)}";
				}
				else
				{
					returnUrl = await ControllerContext.HttpContext.EncryptedActionUrl(nameof(IUiFlowController.Init), flowName, new { source });
				}
			}
			else if (!returnUrl.StartsWith("/") && !returnUrl.StartsWith(HttpContext.GetBaseUrl(), StringComparison.InvariantCultureIgnoreCase))
			{
				return await Login(null, new LoginViewModel());
			}
			else if (returnUrl.StartsWith("/"))
			{
				returnUrl = $"{HttpContext.GetBaseUrl()}{returnUrl}";
			}
			
			return Redirect(returnUrl);
		}

		[IgnoreAntiforgeryToken]
		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel viewModel)
		{
			viewModel = await _encryptionService.DecryptModelAsync(viewModel);
			return await this.HandleActionAsync(async () =>
				{
					await _domainCommandDispatcher.ExecuteAsync(new CreateUserSessionCommand(viewModel.LoginFormData.UserName.AdaptToSapUserNameFormat(),
						viewModel.LoginFormData.Password));

					return await ResolveRedirectionResult(viewModel.ReturnUrl, viewModel.Source);
				},
				 async x => await Login(viewModel.ReturnUrl, viewModel),
				viewModel,
				ex =>
				{
					if (ex.DomainError.Equals(ResidentialDomainError.AuthenticationError))
						ModelState.AddModelError(string.Empty,
							"Incorrect email address and/or password. Please try again or click Forgot Password above to reset it.");
					else
						ModelState.AddModelError(string.Empty, "Error, please try again.");
				});
		}


		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			return await this.HandleActionAsync(
				async () =>
				{
					await _userSessionProvider.EndCurrentSession();
					return RedirectToAction(nameof(Login));
				});
		}

		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> AccessDenied()
		{
			return await this.HandleActionAsync(
				async () => { return Ok("TODO AccessDenied"); });
		}
	}
}