using System.ComponentModel.DataAnnotations;
using S3.CoreServices.System.FastReflection;

namespace S3.Mvc.Core.ViewModels.Validations
{
    public class CustomCompareAttribute : ValidationAttribute
    {
        private string IfPropertyPath { get; }

        public CustomCompareAttribute(params string[] propertyPath)
        {
            var ifPropertyNameDotSeparatedValuesFromModel = string.Join('.', propertyPath);

            this.IfPropertyPath = ifPropertyNameDotSeparatedValuesFromModel;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = value;
            var comparisonValue = validationContext.ObjectInstance.GetPropertyValueFastFromPropertyPath(IfPropertyPath);

            if (currentValue != null && currentValue.Equals(comparisonValue))
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
