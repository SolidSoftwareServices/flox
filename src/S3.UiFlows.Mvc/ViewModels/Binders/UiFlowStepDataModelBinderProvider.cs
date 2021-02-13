using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using NLog;
using S3.CoreServices.Profiling;
using S3.CoreServices.System.DependencyInjection;
using S3.UiFlows.Core.Flows.Screens.Models;
using ILogger = NLog.ILogger;

namespace S3.UiFlows.Mvc.ViewModels.Binders
{

    internal class UiFlowStepDataModelBinderProvider : IModelBinderProvider
    {
	    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<Type, Dictionary<ModelMetadata, IModelBinder>> _propertyBinders =
            new ConcurrentDictionary<Type, Dictionary<ModelMetadata, IModelBinder>>();

        public IModelBinder GetBinder(ModelBinderProviderContext context)
		{
			if (typeof(UiFlowScreenModel).IsAssignableFrom(context.Metadata.ModelType))
			{
				using (context.Services.Resolve<IProfiler>().RecordStep($"{GetType().Name}.{nameof(GetBinder)}"))
				{

					return new UiFlowStepDataModelBinder(context.Services.Resolve<ILoggerFactory>(),t =>
					{
						return _propertyBinders.GetOrAdd(t, type =>
						{
							Logger.Debug(()=>$"Started adding model binder for {type}");
							var propertyBinders = new Dictionary<ModelMetadata, IModelBinder>();
							var metadata = context.MetadataProvider.GetMetadataForType(type);

							foreach (var property in metadata.Properties)
							{
								Logger.Debug(()=>$"Started adding property binder for {type}.{property.Name}");
								propertyBinders.Add(property, context.CreateBinder(property));
								Logger.Debug(()=>$"Added property binder for {type}.{property.Name}");
							}
							Logger.Debug(()=>$"Added model binder for {type}");
							return propertyBinders;
						});
					});
				}
			}

            return null;

           
        }
    }
}
