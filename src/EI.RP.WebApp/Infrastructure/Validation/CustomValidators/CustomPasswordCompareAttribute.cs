using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EI.RP.WebApp.Infrastructure.Validation.CustomValidators
{
    public class CustomPasswordCompareAttribute : System.ComponentModel.DataAnnotations.CompareAttribute, IClientModelValidator
    { 
        private readonly string _errorMessage;
        private readonly string _type;
        private readonly string _otherValue;

        public CustomPasswordCompareAttribute(string errorMessage, string otherValue, string type)
            : base(otherValue)
        {
            _otherValue = otherValue;
            _type = type;
            _errorMessage = errorMessage;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(this.OtherProperty);
            if (property == null)
            {
                return new ValidationResult(string.Format(CultureInfo.CurrentCulture, "Unknown property {0}", this.OtherProperty));
            }
            var otherValue = property.GetValue(validationContext.ObjectInstance, null) as string;
            if (_type == "nocase")
            {
                if (string.Equals(value as string, otherValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    return ValidationResult.Success;
                }
            }
            else if (_type == "mismatch")
            {
                if (!string.Equals(value as string, otherValue))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
        }
        public override string FormatErrorMessage(string name)
        {
            return $"{_errorMessage}";
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-required", errorMessage);
        }

        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }
    
}