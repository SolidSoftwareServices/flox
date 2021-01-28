using System;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.Http.Server;
using EI.RP.CoreServices.Platform;
using EI.RP.CoreServices.System.DependencyInjection;
using EI.RP.DomainServices.Infrastructure.Settings;
using EI.RP.WebApp.Infrastructure.StringResources;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EI.RP.WebApp.Infrastructure.Validation.CustomValidators
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple =
		false)]
	public class ResidentialPortalUserName : ValidationAttribute, IClientModelValidator
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var result = ValidationResult.Success;
		
			var settings = validationContext.Resolve<IDomainSettings>();
			if (settings.IsInternalDeployment)
			{
				if (!new RequiredAttribute().IsValid(value))
				{
					result= new ValidationResult(ReusableString.RequiredUserName);
				}
			}
			else
			{
				if (!new RequiredAttribute().IsValid(value) || !new EmailAddressAttribute().IsValid(value))
				{
					result= new ValidationResult(ReusableString.RequiredEmail);
				}
			}

			return result;

		}

		public void AddValidation(ClientModelValidationContext context)
		{
			context.AddValidation(this);
		}
	}
}