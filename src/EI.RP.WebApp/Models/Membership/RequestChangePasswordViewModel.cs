using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ei.Rp.Mvc.Core.ViewModels;
using EI.RP.WebApp.Infrastructure.StringResources;
using EI.RP.WebApp.Infrastructure.Validation.CustomValidators;

namespace EI.RP.WebApp.Models.Membership
{
	public class RequestChangePasswordViewModel : ChangePasswordViewModel
	{
		public string PotText { get; set; } = string.Empty;
		public string StatusCode { get; set; }
        public string TemporalPassword { get; set; }
        public string Email { get; set; }

		public FormData RequestChangePasswordFormData { get; set; }

        public class FormData
        {
	        [Required(AllowEmptyStrings = false, ErrorMessage = ReusableString.RequiredPassword)]
	        [RegularExpression(ReusableRegexPattern.RegexPassword, ErrorMessage = ReusableString.RequiredValidPassword)]
	        public string NewPassword { get; set; }
        }
}
}