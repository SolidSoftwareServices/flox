using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Diagnostics;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.FastReflection;

namespace Ei.Rp.Mvc.Core.ViewModels.Validations
{
#if !FrameworkDeveloper
    [DebuggerStepThrough]
#endif


public class RequiredIfAttribute : ValidationAttribute
    {
	    private readonly SourceType _sourceType;

	    public enum SourceType
	    {
			Property,BooleanMethod
	    }

        private string Path { get; set; }
        public object[] IfValues { get; set; }

        public object IfValue
        {
            get => IfValues.Single();
            set => IfValues = value?.ToOneItemArray() ?? new object[] { null };
        }

        public RequiredIfAttribute(params string[] path):this(SourceType.Property,path)
        {
        }
        public RequiredIfAttribute(SourceType sourceType, params string[] path)
        {
	        if (_sourceType == SourceType.BooleanMethod && path.Length != 1)
	        {
		        throw new NotSupportedException($"Boolean method source nested not supported");
	        }

	        _sourceType = sourceType;
			
	        var ifPropertyNameDotSeparatedValuesFromModel = string.Join('.', path);

	        this.Path = ifPropertyNameDotSeparatedValuesFromModel;
        }

		protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var sourceValueToCompare = GetSourceValueToCompare();
            bool isValid = false;

			if (value is bool)
            {
                if (!(bool)value)
                {
                    isValid = true;
                }
            }
            else if (value is string && string.IsNullOrWhiteSpace((string)value))
            {
                isValid = true;
            }
            else if (value == null)
            {
                isValid = true;
            }

			if (isValid && IfValues.Any(x => (x == null && sourceValueToCompare == null) || (x != null && x.Equals(sourceValueToCompare))))
			{
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;

            object GetSourceValueToCompare()
            {
	            object result;
	            switch (_sourceType)
	            {
		            case SourceType.Property:
			            result= context.ObjectInstance.GetPropertyValueFastFromPropertyPath(Path);
						break;
		            case SourceType.BooleanMethod:
						//TODO: OPTIMIZE
			            var method = context.ObjectInstance.GetType().GetMethod(Path);
						if (method.ReturnType != typeof(bool))
						{
							throw new InvalidOperationException($"{method.Name} is not boolean");

						}
						if (method.GetParameters().Any())
						{
							throw new InvalidOperationException($"{method.Name} cannot have arguments");

						}

						result = method.Invoke(context.ObjectInstance, new object[] { });
			            break;
		            default:
			            throw new ArgumentOutOfRangeException();
	            }

	            return result;
            }
        }
    }

  
}