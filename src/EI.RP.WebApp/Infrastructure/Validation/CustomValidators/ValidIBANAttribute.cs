using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EI.RP.WebApp.Infrastructure.Validation.CustomValidators
{
	public class ValidIBANAttribute : RegularExpressionAttribute, IClientModelValidator
	{		
		private string _displayName;
		private readonly string _checkSumComparerPropertyName;
		private readonly string _errorMessage;
		private static Regex IsAlphaNumericRegex = new Regex("^[a-zA-Z0-9]+$");

		/// <summary>
		/// 
		/// </summary>
		/// <param name="errorMessage"></param>
		/// <param name="checkSumComparerPropertyName">when specified validates the checksum agains the given property</param>
		public ValidIBANAttribute( string errorMessage, string checkSumComparerPropertyName=null):base(@"^([Ii][Ee][A-Za-z0-9\*]{20})$")
		{
			_checkSumComparerPropertyName = checkSumComparerPropertyName;
			_errorMessage = errorMessage;
		}
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			_displayName = validationContext.DisplayName;
			var strValue = value as string;

			if (string.IsNullOrWhiteSpace(strValue))
			{
				return ValidationResult.Success;
			}

			if (IsAlphaNumericRegex.IsMatch(strValue))
			{
                if (_checkSumComparerPropertyName == null || ValidateCheckSum(validationContext, strValue))
                {
                    var baseResult = base.IsValid(value, validationContext);
                  
                    return baseResult;
                }
			}

			return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
		}

	
		private bool ValidateCheckSum(ValidationContext validationContext, string strValue)
		{
			var reminder = 0;

			var iban = strValue.All(x => x != '*')
				? strValue.Replace(" ", "").ToUpper()
				: validationContext.ObjectType.GetProperty(_checkSumComparerPropertyName)
					.GetValue(validationContext.ObjectInstance, null)
					.ToString().Replace(" ", "").ToUpper();


            if(iban.Length > 4)
                iban = iban.Substring(4) + iban.Substring(0, 4);
			var convertedIban = string.Empty;
			for (var j = 0; j < iban.Length; ++j)
			{
				if (!char.IsDigit(iban[j]))
					convertedIban += (iban[j]-55).ToString();
				else
					convertedIban += iban[j];
			}


			while (convertedIban.Length > 0)
			{
				var p = convertedIban.Length > 7 ? 7 : convertedIban.Length;
				reminder = int.Parse(reminder.ToString() + convertedIban.Substring(0, p)) % 97;
				convertedIban = convertedIban.Substring(p);
			}

			return reminder == 1;
		}


		public override string FormatErrorMessage(string name)
		{
			return $"{_errorMessage}";
		}

		public void AddValidation(ClientModelValidationContext context)
		{
			context.AddValidation(this);
		}
	}
}