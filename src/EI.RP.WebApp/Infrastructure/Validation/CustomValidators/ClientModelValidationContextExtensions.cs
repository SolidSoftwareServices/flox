using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EI.RP.WebApp.Infrastructure.Validation.CustomValidators
{
	public static class ClientModelValidationContextExtensions
	{
		public static void AddValidation(this ClientModelValidationContext context,ValidationAttribute validationAttribute)
		{
			context.Attributes.AddSafe("data-val", true.ToString());
			var errorMessage = validationAttribute.FormatErrorMessage(context.ModelMetadata.GetDisplayName());
			context.Attributes.AddSafe("data-val-required", errorMessage);
		}

	}
}