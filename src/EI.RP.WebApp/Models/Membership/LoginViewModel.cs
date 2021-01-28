using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ei.Rp.Mvc.Core.ViewModels;
using EI.RP.WebApp.Infrastructure.StringResources;
using EI.RP.WebApp.Infrastructure.Validation.CustomValidators;

namespace EI.RP.WebApp.Models.Membership
{
    public class LoginViewModel:IErrorsViewModel
	{
		public string PotText { get; set; } = string.Empty;
		public IEnumerable<string> ErrorMessages { get; set; }

		public string Source { get; set; }
		public string ReturnUrl { get; set; }

		public FormData LoginFormData { get; set; }

		public class FormData
		{
			[ResidentialPortalUserName]
			public string UserName { get; set; }

			[Required(ErrorMessage = ReusableString.RequiredPassword)]
			public string Password { get; set; }
		}
    }
}
