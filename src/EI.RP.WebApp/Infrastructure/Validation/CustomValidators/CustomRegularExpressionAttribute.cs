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
    public class CustomRegularExpressionAttribute : RegularExpressionAttribute, IClientModelValidator
    { 
        private readonly string _errorMessage;

        public CustomRegularExpressionAttribute(string errorMessage, string regexKey)
            : base(regexKey)
        {
            _errorMessage = errorMessage;
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