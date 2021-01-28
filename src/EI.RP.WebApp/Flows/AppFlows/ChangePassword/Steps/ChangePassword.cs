using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.System;
using EI.RP.WebApp.Flows.AppFlows.ChangePassword.FlowDefinitions;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DomainServices.Commands.Users.Membership.ChangePassword;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Infrastructure.Validation.CustomValidators;
using Ei.Rp.DomainErrors;

namespace EI.RP.WebApp.Flows.AppFlows.ChangePassword.Steps
{
    public class ChangePassword : ChangePasswordScreen
    {
	    private readonly IUserSessionProvider _userSessionProvider;
        private readonly IDomainCommandDispatcher _domainCommandDispatcher;

        public ChangePassword(
	        IUserSessionProvider userSessionProvider,
	        IDomainCommandDispatcher domainCommandDispatcher)
	   {
           _userSessionProvider = userSessionProvider;
           _domainCommandDispatcher = domainCommandDispatcher;
	   }

        public static class StepEvent
        {
            public static readonly ScreenEvent SaveNewPassword = new ScreenEvent(nameof(ChangePassword),nameof(SaveNewPassword));
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
            IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
                .OnEventReentriesCurrent(StepEvent.SaveNewPassword);
        }

        protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
        {
            var userName = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name.Trim();
            var stepData = contextData.GetStepData<RequestNewPasswordScreenModel>();

            if (triggeredEvent == StepEvent.SaveNewPassword)
            {
                stepData.WasPasswordTriedToChange = true;
               
                await _domainCommandDispatcher.ExecuteAsync(new ChangePasswordCommand(
                    userName: userName,
                    currentPassword: stepData.CurrentPassword,
                    newPassword: stepData.NewPassword,
                    activationKey: null, 
                    activationStatus: null,
                    requestId: null));
                stepData.IsPasswordChangedSuccess = true;
            }
        }

        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
            UiFlowScreenModel originalScreenModel,
            IDictionary<string, object> stepViewCustomizations = null)
        {
	        var refreshedStepData = originalScreenModel.CloneDeep();
            var updatedStepData = (RequestNewPasswordScreenModel)refreshedStepData;
            if (!updatedStepData.IsPasswordChangedSuccess && contextData.LastError?.ExceptionMessage != null)
            {
	            var lastErrorMessage = contextData.LastError.ExceptionMessage;

				if (lastErrorMessage.StartsWith(DomainErrorText(ResidentialDomainError.NewPasswordMustBeDifferentThanTheLast5Passwords)))
                {
                    updatedStepData.ErrorMessage = ResidentialDomainError.NewPasswordMustBeDifferentThanTheLast5Passwords.ErrorMessage;
                }
                else if (lastErrorMessage.StartsWith(DomainErrorText(ResidentialDomainError.ChangePasswordOncePerDayError)))
                {
                    updatedStepData.ErrorMessage = ResidentialDomainError.ChangePasswordOncePerDayError.ErrorMessage;
                }
                else if (lastErrorMessage.StartsWith(DomainErrorText(ResidentialDomainError.IncorrectPasswordError)))
                {
	                updatedStepData.ErrorMessage = ResidentialDomainError.IncorrectPasswordError.ErrorMessage;
                }
				else
                {
                    updatedStepData.ErrorMessage = lastErrorMessage;
                }
            }

            SetTitle(Title, updatedStepData);

            string DomainErrorText(DomainError error)
            {
	            return $"{error.ErrorCode} - {error.ErrorMessage} -";
            }

            return updatedStepData;
        }

        public override ScreenName ScreenStep => ChangePasswordStep.ChangePassword;

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
	        var screenModel = new RequestNewPasswordScreenModel();

	        SetTitle(Title, screenModel);

	        return screenModel;
        }

        public class RequestNewPasswordScreenModel : UiFlowScreenModel
        {
            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == ChangePasswordStep.ChangePassword;
            }            
            public string AccountNumber { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a password")]
            public string CurrentPassword { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a password")]
            [CustomPasswordCompare("Old and new password should not be same","CurrentPassword", "mismatch")]
            [CustomRegularExpressionAttribute("The new password must be at least 8 characters long and contain at least one number, " +
                                              "one upper case, and one lower case letter. The new password cannot be one of the last 5 passwords used", @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,40}$")]
            public string NewPassword { get; set; }
            public string ErrorMessage { get; set; }
            public bool IsPasswordChangedSuccess { get; set; }
            public bool WasPasswordTriedToChange { get; set; }
        }
    }
}