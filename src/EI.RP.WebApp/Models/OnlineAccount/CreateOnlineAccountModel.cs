using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Ei.Rp.Mvc.Core.ViewModels;
using EI.RP.WebApp.Infrastructure.StringResources;

namespace EI.RP.WebApp.Models.OnlineAccount
{
	public class CreateOnlineAccountModel : IErrorsViewModel
	{
		public string PotText { get; set; } = string.Empty;

		public IEnumerable<string> ErrorMessages { get; set; }

		public string Source { get; set; }

		public FormData CreateOnlineAccountFormData { get; set; }

		public class FormData
		{
			[RegularExpression(ReusableRegexPattern.RegexAccountNumber, ErrorMessage = ReusableString.ValidAccountNumber)]
			[Required(ErrorMessage = ReusableString.RequiredAccountNumber)]
			public string AccountNumber { get; set; }

			[Required(ErrorMessage = ReusableString.ValidMprnGprn)]
			[RegularExpression(ReusableRegexPattern.RegexShortMPRNGPRNNumber, ErrorMessage = ReusableString.ValidMprnGprn)]
			public string MPRNGPRN { get; set; }

			[Required(ErrorMessage = ReusableString.RequiredEmail)]
			[MaxLength(64, ErrorMessage = ReusableString.EmailTooLong)]
			public string Email { get; set; }

			[Required(ErrorMessage = ReusableString.RequiredPhoneNumber)]
			[RegularExpression(ReusableRegexPattern.ValidPhoneNumber, ErrorMessage = ReusableString.RequiredPhoneNumber)]
			[MaxLength(ReusableString.MaxLengthPhoneNumber, ErrorMessage = ReusableString.RequiredPhoneNumber)]
			[MinLength(ReusableString.MinLengthPhoneNumber, ErrorMessage = ReusableString.MinLengthPhoneNumberErrorMessage)]
			public string PhoneNumber { get; set; }

			[Range(typeof(bool), "true", "true", ErrorMessage="Please confirm that you have read and accept the Electric Ireland Terms and Conditions")]
			public bool TermsAndConditionsAccepted { get; set; }

			[Range(typeof(bool), "true", "true", ErrorMessage="Please accept the permission")]
			public bool IsAccountOwner { get; set; }

			[Required(ErrorMessage = "Please enter a date of birth")]
			public int? DateOfBirthDay { get; set; }

			[Required(ErrorMessage = "Please enter a date of birth")]
			public int? DateOfBirthMonth { get; set; }

			[Required(ErrorMessage = "Please enter a date of birth")]
			public int? DateOfBirthYear { get; set; }

			[Required(ErrorMessage = "Please enter a valid date of birth")]
			[DataType(DataType.Date, ErrorMessage = "Please enter a valid date of birth")]
			public DateTime? DateOfBirth {
				get
				{
					if (DateOfBirthYear >= 1900 &&  DateTime.TryParse($"{DateOfBirthYear}-{DateOfBirthMonth}-{DateOfBirthDay}", out var dob) && (dob.AddYears(18).Date <= DateTime.Today))
					{
						return dob;
					}

					return null;
				}
			}
		}
	}
}