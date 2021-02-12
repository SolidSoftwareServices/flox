using System;
using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Autofac.Features.Scanning;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using S3.CoreServices.Profiling;

namespace S3.CoreServices.IoC.Autofac
{
	public static class AutofacExtensions
	{
		public static IServiceCollection RegisterAutofacContainer(this IServiceCollection services)
		{
			var builder = new ContainerBuilder();
			builder.Populate(services);
			return services;
		}


		//public static IRegistrationBuilder<TLimit, TConcreteActivatorData, SingleRegistrationStyle>
		//	WithProfiling<TLimit, TConcreteActivatorData>(
		//		this IRegistrationBuilder<TLimit, TConcreteActivatorData, SingleRegistrationStyle> registration)
		//{
		//	if (!DetailedProfilingEnabled) return registration;
		//	return registration.EnableInterfaceInterceptors().InterceptedBy(typeof(ProfilerInterceptor));
		//}

		
		public static bool DetailedProfilingEnabled { get; set; }

		
        public static IRegistrationBuilder<TLimit, ScanningActivatorData, TRegistrationStyle> WithClassProfiling<TLimit, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, ScanningActivatorData, TRegistrationStyle> registration)
        {
	        if (!DetailedProfilingEnabled) return registration;
	        return registration.EnableClassInterceptors().InterceptedBy(typeof(ProfilerInterceptor));
        }

      
        public static IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle> WithClassProfiling<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle> registration)
            where TConcreteReflectionActivatorData : ConcreteReflectionActivatorData
        {
	        if (!DetailedProfilingEnabled) return registration;
	        return registration.EnableClassInterceptors().InterceptedBy(typeof(ProfilerInterceptor));
        }

     
        public static IRegistrationBuilder<TLimit, ScanningActivatorData, TRegistrationStyle> WithClassProfiling<TLimit, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, ScanningActivatorData, TRegistrationStyle> registration,
            ProxyGenerationOptions options,
            params Type[] additionalInterfaces)
        {
	        if (!DetailedProfilingEnabled) return registration;
	        return registration.EnableClassInterceptors().InterceptedBy(typeof(ProfilerInterceptor));
        }

      
        public static IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle> WithInterfaceProfiling<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle> registration,
            ProxyGenerationOptions options,
            params Type[] additionalInterfaces)
            where TConcreteReflectionActivatorData : ConcreteReflectionActivatorData
        {
	        if (!DetailedProfilingEnabled) return registration;
	        return registration.EnableInterfaceInterceptors().InterceptedBy(typeof(ProfilerInterceptor));
        }

       
        public static IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle> WithInterfaceProfiling<TLimit, TActivatorData, TSingleRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle> registration, ProxyGenerationOptions options = null)
        {
	        if (!DetailedProfilingEnabled) return registration;
	        return registration.EnableInterfaceInterceptors().InterceptedBy(typeof(ProfilerInterceptor));
        }

        
	}
}
