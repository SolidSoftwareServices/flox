using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using S3.CoreServices.System;
using S3.Mvc.Core.ViewModels.Binders;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Mvc.ViewModels.Binders
{

	internal class UiFlowStepDataModelBinder : ModelBinderBase
	{
		private readonly Func<Type, Dictionary<ModelMetadata, IModelBinder>> _propsBinderFactory;
		private readonly ILoggerFactory _loggerFactory;

		public UiFlowStepDataModelBinder(ILoggerFactory loggerFactory,
			Func<Type, Dictionary<ModelMetadata, IModelBinder>> propsBinderFactory)
		{
			_propsBinderFactory = propsBinderFactory;
			_loggerFactory = loggerFactory;
		}

		protected override async Task _Execute(ModelBindingContext bindingContext,
			EncryptedPropertyDataValueProvider valueProvider)
		{

			var result = ModelBindingResult.Failed();

			var valueProviderResult = await valueProvider
				.GetValueAsync(ModelNames.CreatePropertyModelName(bindingContext.ModelName,
					nameof(UiFlowScreenModel.FlowScreenName)));
			var modelTypeValue =valueProviderResult.FirstValue;

			if (modelTypeValue != null)
			{
				var modelType = TypesFinder.Resolver.FindType(modelTypeValue);
				var propertyBinders = _propsBinderFactory(modelType);
				var modelBinder = new ComplexTypeModelBinder(propertyBinders, _loggerFactory);
				result = await _BindModel(bindingContext, modelType, valueProvider,modelBinder);

			}

			bindingContext.Result = result;
		}
	}
}