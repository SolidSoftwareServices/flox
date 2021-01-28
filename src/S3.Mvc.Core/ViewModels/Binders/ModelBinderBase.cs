using System;
using System.Diagnostics;
using System.Threading.Tasks;
using S3.CoreServices.Encryption;
using S3.CoreServices.Profiling;
using S3.Mvc.Core.System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NLog;

namespace S3.Mvc.Core.ViewModels.Binders
{
#if !FrameworkDeveloper
    [DebuggerStepThrough]
#endif
	public abstract class ModelBinderBase : IModelBinder
	{

		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		public async Task BindModelAsync(ModelBindingContext bindingContext)
		{
			var valueProvider = new EncryptedPropertyDataValueProvider(bindingContext.ValueProvider, bindingContext.HttpContext.Resolve<IEncryptionService>());
			using (bindingContext.HttpContext.Resolve<IProfiler>()
				.RecordStep($"{nameof(ModelBinderBase)}.{nameof(BindModelAsync)}"))
			{
				Logger.Trace(()=>nameof(BindModelAsync));
				await _Execute(bindingContext,valueProvider);
			}
		}

		protected abstract Task _Execute(ModelBindingContext bindingContext,
			EncryptedPropertyDataValueProvider valueProvider);

		protected async Task<ModelBindingResult> _BindModel(ModelBindingContext bindingContext, Type modelType,
			EncryptedPropertyDataValueProvider valueProvider, IModelBinder innerBinder)
		{

			var innerModelBindingContext = DefaultModelBindingContext.CreateBindingContext(
				bindingContext.ActionContext,
				valueProvider,
				bindingContext.HttpContext.Resolve<IModelMetadataProvider>().GetMetadataForType(modelType),
				null,
				bindingContext.ModelName);

			await innerBinder.BindModelAsync(innerModelBindingContext);

			var modelBindingResult = innerModelBindingContext.Result;
			if (modelBindingResult == ModelBindingResult.Failed())
			{
				Logger.Error(() => $"ModelBinder of {bindingContext.ModelName} failed");

			}

			return modelBindingResult;
		}
	}
}