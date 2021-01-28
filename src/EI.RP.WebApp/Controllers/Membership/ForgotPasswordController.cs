using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.Http.Session;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.Membership;
using EI.RP.DomainServices.Commands.Users.Membership.ChangePassword;
using EI.RP.DomainServices.Commands.Users.Membership.ResetPassword;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.DomainServices.Queries.Membership.ForgotPasswordRequestResults;
using Ei.Rp.Mvc.Core.Controllers;
using EI.RP.WebApp.Models.Membership;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using ControllerBase = Ei.Rp.Mvc.Core.Controllers.ControllerBase;
using System;

namespace EI.RP.WebApp.Controllers.Membership
{
    public class ForgotPasswordController : ControllerBase
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDomainCommandDispatcher _domainCommandDispatcher;
        private readonly IDomainQueryResolver _domainQueryProvider;
        private readonly IUserSessionProvider _userSessionProvider;
        private readonly IEncryptionService _encryptionService;

        public ForgotPasswordController(IDomainCommandDispatcher domainCommandDispatcher, IDomainQueryResolver domainQueryProvider,
            IUserSessionProvider userSessionProvider, IEncryptionService encryptionService)
        {
            _domainCommandDispatcher = domainCommandDispatcher;
            _domainQueryProvider = domainQueryProvider;
            _userSessionProvider = userSessionProvider;
            _encryptionService = encryptionService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Login/ForgotPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            model = await _encryptionService.DecryptModelAsync(model);

            return View("RequestResetPassword", model);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login/ForgotPassword")]
        public async Task<IActionResult> RequestResetPassword(ResetPasswordViewModel viewModel)
        {
            viewModel = await _encryptionService.DecryptModelAsync(viewModel);

            return await this.HandleActionAsync(async () =>
	            {
		            await _domainCommandDispatcher.ExecuteAsync(
			            new ResetPasswordDomainCommand(viewModel.ResetPasswordFormData.Email.AdaptToSapUserNameFormat()));
		            return View("ResetPasswordConfirmationEmail", viewModel);
	            },
	            async x => await ResetPassword(viewModel),
	            viewModel,
	            ex =>
	            {

		            Logger.Debug(() => "Error");
		            ModelState.AddModelError(
			            string.Empty,
						"Sorry, an error occurred while processing your request.");

	            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Login/ForgetPassword")]
        public async Task<IActionResult> ChangePasswordCallback(string requestId, string activationKey)
        {
            return RedirectToAction(
                nameof(ChangePassword),
                new ChangePasswordViewModel
                {
                    RequestId = requestId,
                    ActivationKey = activationKey
                });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Login/ForgotPassword/Create")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            viewModel = await _encryptionService.DecryptModelAsync(viewModel);

            return await this.HandleActionAsync(async () =>
                {
                    try
                    {
                        var result = (await _domainQueryProvider
                            .FetchAsync<ForgotPasswordRequestResultQuery, ForgotPasswordRequestResult>(
                                new ForgotPasswordRequestResultQuery
                                {
                                    RequestId = viewModel.RequestId
                                })
                            ).SingleOrDefault();

                        if (result.IsValid)
                        {
                            return await OnChangePassword(new RequestChangePasswordViewModel
                            {
                                Email = result.Email,
                                TemporalPassword = result.TemporalPassword,
                                RequestId = viewModel.RequestId,
                                ActivationKey = viewModel.ActivationKey,
								StatusCode = result.StatusCode
                            });
                        }
                        else
                        {
                            return RedirectToAction(nameof(LinkExpired));
                        }
                    }
                    catch (Exception)
                    {
                        return RedirectToAction(nameof(LinkExpired));
                    }
                },
                nameof(ChangePassword),
                viewModel,
                ex =>
                {
                    Logger.Debug(() => "Error");
                    ModelState.AddModelError(
                        string.Empty,
                        "Sorry, an error occurred while processing your request.");
                });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login/ForgetPassword/Create")]
        public async Task<IActionResult> RequestChangePassword(RequestChangePasswordViewModel viewModel)
        {
            viewModel = await _encryptionService.DecryptModelAsync(viewModel);

            return await this.HandleActionAsync(async () =>
            {
                var result = (await _domainQueryProvider
                    .FetchAsync<ForgotPasswordRequestResultQuery, ForgotPasswordRequestResult>(
                        new ForgotPasswordRequestResultQuery
                        {
                            RequestId = viewModel.RequestId
                        })
                    ).SingleOrDefault();

                if (result.IsValid)
                {
                    await _domainCommandDispatcher.ExecuteAsync(new ChangePasswordCommand(
                        userName: viewModel.Email.AdaptToSapUserNameFormat(),
                        currentPassword: viewModel.TemporalPassword,
                        newPassword: viewModel.RequestChangePasswordFormData.NewPassword,
                        activationKey: viewModel.ActivationKey,
						activationStatus: result.StatusCode,
						requestId: viewModel.RequestId));

                    return RedirectToAction(
                        nameof(LoginController.Login),
                        typeof(LoginController).GetNameWithoutSuffix());
                }
                else
                {
                    return RedirectToAction(nameof(LinkExpired));
                }
            },
            async x => await OnChangePassword(viewModel),
            viewModel,
            ex =>
            {
                if (ex.DomainError.Equals(ResidentialDomainError.NewPasswordMustBeDifferentThanTheLast5Passwords))
                {
                    Logger.Debug(() => "Error");
                    ModelState.AddModelError(string.Empty, "Please use a different password. Your new password cannot be the same as your previous 5 passwords.");
                }
                else
                {
                    Logger.Debug(() => "Error");
                    ModelState.AddModelError(string.Empty, "An error has occurred, please try again later.");
                }
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Login/ForgotPassword/LinkExpired")]
        public async Task<IActionResult> LinkExpired()
        {
            return View("ResetPasswordLinkExpired");
        }

        public async Task<IActionResult> OnChangePassword(RequestChangePasswordViewModel viewModel)
        {
            viewModel = await _encryptionService.DecryptModelAsync(viewModel);
            return View("ForceChangePassword", viewModel);
        }
    }
}