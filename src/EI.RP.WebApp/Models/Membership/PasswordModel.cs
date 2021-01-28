using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ei.Rp.Mvc.Core.ViewModels;
using EI.RP.WebApp.Infrastructure.StringResources;

namespace EI.RP.WebApp.Models.Membership
{
	public class PasswordModel : IErrorsViewModel
	{
		public string PotText { get; set; } = string.Empty;
		
		public string ActivationKey { get; set; }

		public string RequestId { get; set; }

		public string Email { get; set; }

		public IEnumerable<string> ErrorMessages { get; set; }

		public FormData PasswordFormData { get; set; }

		public class FormData 
		{
			[Required(ErrorMessage = ReusableString.RequiredPassword)]
			[RegularExpression(ReusableRegexPattern.RegexPassword, ErrorMessage = ReusableString.RequiredValidPassword)]
			public string Password { get; set; }

			[Required(ErrorMessage = ReusableString.RequiredPassword)]
			[Compare("Password", ErrorMessage = "The passwords need to match")]
			public string ConfirmPassword { get; set; }

			[Required(ErrorMessage = "Please enter a date of birth")]
			public int? DateofBirthDay { get; set; }

			[Required(ErrorMessage = "Please enter a date of birth")]
			public int? DateofBirthMonth { get; set; }

			[Required(ErrorMessage = "Please enter a date of birth")]
			public int? DateofBirthYear { get; set; }

			[Required(ErrorMessage = "Please enter a valid date")]
			[DataType(DataType.Date, ErrorMessage = "Please enter a valid date")]
			public DateTime? DateOfBirth
			{
				get
				{
					if (DateTime.TryParse($"{DateofBirthYear}-{DateofBirthMonth}-{DateofBirthDay}", out var dob))
					{
						return dob;
					}

					return null;
				}
			}
		}
	}
}
