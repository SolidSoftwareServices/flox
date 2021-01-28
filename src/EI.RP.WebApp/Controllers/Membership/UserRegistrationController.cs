using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.Mvc.Core.Controllers;
using EI.RP.WebApp.Models.Membership;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using ControllerBase = Ei.Rp.Mvc.Core.Controllers.ControllerBase;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Membership;
using System.Linq;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainErrors;
using EI.RP.DomainServices.Commands.Users.Membership.CreatePassword;
using EI.RP.DomainServices.Queries.Membership.CreatePasswordRequestResults;

namespace EI.RP.WebApp.Controllers.Membership
{
    public class UserRegistrationController : ControllerBase
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDomainCommandDispatcher _domainCommandDispatcher;
        private readonly IDomainQueryResolver _domainQueryProvider;
        private readonly IEncryptionService _encryptionService;

        public UserRegistrationController(IDomainCommandDispatcher domainCommandDispatcher, IDomainQueryResolver domainQueryProvider,
         IEncryptionService encryptionService)
        {
            _domainCommandDispatcher = domainCommandDispatcher;
            _domainQueryProvider = domainQueryProvider;
            _encryptionService = encryptionService;
        }

        [AllowAnonymous]
		[HttpGet]
        [Route("Login/ActivateRegistration")]
        public async Task<IActionResult> ActivateRegistration(PasswordModel model)
        {
	        return await this.HandleActionAsync(async () =>
	            {
		            if (string.IsNullOrEmpty(model.RequestId))
		            {
			            return LinkExpiredOrInvalidView();
		            }

		            model.RequestId = await _encryptionService.DecryptAsync(model.RequestId, true);
		            model.ActivationKey = await _encryptionService.DecryptAsync(model.ActivationKey, true);

		            var result = await GetCreatePasswordRequest(model.RequestId);

					if (!result.Valid)
					{
						return LinkExpiredOrInvalidView();
					}

					model.Email = result.Email;
					return View("CreatePassword", model);
	            },
				nameof(ActivateRegistration),
				model);
        }

        [AllowAnonymous]
		[HttpPost]
		[Route("Login/ActivateRegistration")]
        public async Task<IActionResult> SaveNewPassword(PasswordModel model)
        {
			var success = false;
			var expired = false;

	        model = await _encryptionService.DecryptModelAsync(model);

	        var actionResult = await this.HandleActionAsync(async () =>
		        {
			        var result = await GetCreatePasswordRequest(model.RequestId);

			        if (!result.Valid)
			        {
				        expired = true;
				        return LinkExpiredOrInvalidView(); 
			        }

			        if (result.DateOfBirth != model.PasswordFormData.DateOfBirth)
			        {
				        ModelState.AddModelError($"{nameof(model.PasswordFormData)}.{nameof(model.PasswordFormData.DateOfBirth)}", 
					        "Date of birth does not match");
				        return View("CreatePassword", model);
					}

			        await _domainCommandDispatcher.ExecuteAsync(new CreatePasswordCommand(model.PasswordFormData.ConfirmPassword,
				        model.ActivationKey, model.RequestId, model.Email, model.PasswordFormData.Password));

			        success = true;

			        return RedirectToAction(nameof(LoginController.Login), typeof(LoginController).GetNameWithoutSuffix());
		        },
				nameof(ActivateRegistration),
				model, 
		        ex =>
		        {
			        ModelState.AddModelError(string.Empty,
				        "Sorry there has been an error. Please try again in a few moments.");
		        });

			return success || expired ? actionResult : View("CreatePassword", model);
        }
		
		private ViewResult LinkExpiredOrInvalidView()
		{
			return View("RegistrationLinkExpired");
		}

		private async Task<CreatePasswordResult> GetCreatePasswordRequest(string requestId)
		{
			var valid = false;
			string email = null;
			DateTime? dateOfBirth = null;

			try
			{
				var query = (
					await _domainQueryProvider
						.FetchAsync<CreatePasswordRequestResultQuery, CreatePasswordRequestResult>(
							new CreatePasswordRequestResultQuery 
							{
								RequestId = requestId
							})
				).Single();

				if (query.DateOfBirth?.ToString() != null && query.StatusCode == "00")
				{
					valid = true;
					email = query.Email;
					dateOfBirth = query.DateOfBirth;
				}
			}
			catch (Exception ex)
			{
				var domainException = (DomainException) ex.InnerException;
				if (domainException != null && domainException.DomainError.Equals(ResidentialDomainError.NoDataFound))
				{
					valid = false;
					email = null;
				}
			}

			return new CreatePasswordResult
			{
				Valid = valid, 
				Email = email,
				DateOfBirth = dateOfBirth
			};
		}

		private class CreatePasswordResult
		{
			public bool Valid { get; set; }
			public string Email { get; set; }
			public DateTime? DateOfBirth { get; set; }
		}
    }
}
