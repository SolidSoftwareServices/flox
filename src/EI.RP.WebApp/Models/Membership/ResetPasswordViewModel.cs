using Ei.Rp.Mvc.Core.ViewModels;
using EI.RP.WebApp.Infrastructure.StringResources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.System;

namespace EI.RP.WebApp.Models.Membership
{
    public class ResetPasswordViewModel : IErrorsViewModel
    {
	    public string PotText { get; set; } = string.Empty;

	    public string InvalidEmail { get; set; }

        public IEnumerable<string> ErrorMessages { get; set; }

        public FormData ResetPasswordFormData { get; set; }

        public class FormData
        {
	        [EmailAddress(ErrorMessage = ReusableString.RequiredEmail)]
	        [Required(ErrorMessage = ReusableString.RequiredEmail)]
	        public string Email { get; set; }
        }
    }
}
