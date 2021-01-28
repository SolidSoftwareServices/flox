using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.WebApp.Models.OnlineAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ei.Rp.DomainErrors;
using EI.RP.DomainServices.Commands.Users.Membership.CreateAccount;
using Ei.Rp.Mvc.Core.Controllers;

namespace EI.RP.WebApp.Controllers
{
    public class OnlineAccountController : Ei.Rp.Mvc.Core.Controllers.ControllerBase
    {
        private readonly IDomainCommandDispatcher _domainCommandsDispatcher;

        public OnlineAccountController(IDomainCommandDispatcher domainCommandsDispatcher)
        {
            _domainCommandsDispatcher = domainCommandsDispatcher;
        }

        [AllowAnonymous]
		[Route("Login/SignUp")]
        public async Task<IActionResult> CreateOnlineAccountView(CreateOnlineAccountModel model)
        {
	        return View("CreateOnlineAccount", model);
        }
		 
        [AllowAnonymous]
        [HttpPost]
        [Route("Login/SignUp")]
        public async Task<IActionResult> CreateOnlineAccount(CreateOnlineAccountModel viewModel)
        {
	        var result = await this.HandleActionAsync(async () =>
		        {
			        var form = viewModel.CreateOnlineAccountFormData;

			        await _domainCommandsDispatcher.ExecuteAsync(new CreateAccountCommand(form.AccountNumber,
				        form.Email, form.MPRNGPRN.Trim(), form.PhoneNumber.Trim(), form.DateOfBirth.Value,
				        form.IsAccountOwner, form.TermsAndConditionsAccepted));

			        return View("CreateAccountConfirmationEmail", viewModel);
		        },
		        nameof(CreateOnlineAccountView),
		        viewModel,
		        ex =>
		        {
			        var formDataName = nameof(viewModel.CreateOnlineAccountFormData);

			        if (ex.DomainError.Equals(ResidentialDomainError.UserAlreadyExists))
			        {
				        ModelState.AddModelError(
					        $"{formDataName}.{nameof(viewModel.CreateOnlineAccountFormData.Email)}",
					        "Email address already exists, please enter another email address.");
			        }
			        else if (ex.DomainError.Equals(ResidentialDomainError.Invalid_MPRN))
			        {
				        ModelState.AddModelError(
					        $"{formDataName}.{nameof(viewModel.CreateOnlineAccountFormData.MPRNGPRN)}",
					        "Please enter a valid MPRN or GPRN");
			        }
			        else if (ex.DomainError.Equals(ResidentialDomainError.Invalid_AccountNumber))
			        {
				        ModelState.AddModelError(
					        $"{formDataName}.{nameof(viewModel.CreateOnlineAccountFormData.AccountNumber)}", 
					        "Please enter a valid account number");
			        }
			        else if (ex.DomainError.Equals(ResidentialDomainError.AccountAlreadyExists))
			        {
				        ModelState.AddModelError(
					        $"{formDataName}.{nameof(viewModel.CreateOnlineAccountFormData.AccountNumber)}", 
					        "Registration is not possible because this account number has already been registered by another customer.");
			        }
			        else if (ex.DomainError.Equals(ResidentialDomainError.BusinessAccount))
			        {
				        ModelState.AddModelError(string.Empty, "The details you entered are for a Business Account. Please log in to Business Online to view your account.");
			        }
			        else
			        {
				        ModelState.AddModelError(string.Empty, "Sorry there has been an error. Please try again in a few moments.");
			        }
		        });

	        return result is ViewResult ? result : await CreateOnlineAccountView(viewModel);
        }
    }
}