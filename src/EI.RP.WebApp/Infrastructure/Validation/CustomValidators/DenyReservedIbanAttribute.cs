using System;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.System.DependencyInjection;
using EI.RP.DomainServices.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NLog;

namespace EI.RP.WebApp.Infrastructure.Validation.CustomValidators
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DenyReservedIbanAttribute : ValidationAttribute, IClientModelValidator
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		private readonly string _errorMessage;
		public DenyReservedIbanAttribute(string errorMessage)
		{
			_errorMessage = errorMessage;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (!(value is string)) return ValidationResult.Success;

			var reservedIbanService = validationContext.Resolve<IReservedIbanService>();

			if (reservedIbanService.IsReservedIban((string) value))
			{
				Logger.Debug($"{value} matched reserved Iban");
				return new ValidationResult(_errorMessage);
			}

			return ValidationResult.Success;
		}

		public void AddValidation(ClientModelValidationContext context)
		{
			context.AddValidation(this);
		}
	}
}