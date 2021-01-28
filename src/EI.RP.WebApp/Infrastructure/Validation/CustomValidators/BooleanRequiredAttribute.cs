using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EI.RP.WebApp.Infrastructure.Validation.CustomValidators
{
	[AttributeUsage(AttributeTargets.Property)]
	public class BooleanRequiredAttribute : ValidationAttribute, IClientModelValidator
	{
		public void AddValidation(ClientModelValidationContext context)
		{
			context.AddValidation(this);
		}

		public override bool IsValid(object value)
		{
			return value is bool && (bool) value;
		}
	}
}