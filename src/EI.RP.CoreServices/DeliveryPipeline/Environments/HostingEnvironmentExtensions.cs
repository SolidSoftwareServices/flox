using Microsoft.AspNetCore.Hosting;

namespace EI.RP.CoreServices.DeliveryPipeline.Environments
{
	public static class HostingEnvironmentExtensions
	{
		
		public static bool IsDevelopmentEnv(this IHostingEnvironment src)
		{
			return src.IsEnvironment(AppEnvironment.Development) ||
			       src.IsEnvironment(AppEnvironment.DevelopmentInternal);
		}
		public static bool IsDevelopmentDemoEnv(this IHostingEnvironment src)
		{
			return src.IsEnvironment(AppEnvironment.DevelopmentDemo) ||
			       src.IsEnvironment(AppEnvironment.DevelopmentDemoInternal);
		}

		public static bool IsTest(this IHostingEnvironment src)
		{
			return src.IsEnvironment(AppEnvironment.Test) || src.IsEnvironment(AppEnvironment.CorrectTest) ||
			       src.IsEnvironment(AppEnvironment.TestInternal);
		}

		public static bool IsPreProduction(this IHostingEnvironment src)
		{
			return src.IsEnvironment(AppEnvironment.PreProd) || src.IsEnvironment(AppEnvironment.PreProdInternal);
		}

		public static bool IsProductionEnvironment(this IHostingEnvironment src)
		{
			return src.IsEnvironment(AppEnvironment.Production) || src.IsEnvironment(AppEnvironment.ProductionInternal);
		}

		public static bool IsEnvironmentConfiguredCorrectly(this IHostingEnvironment src)
		{
			return src.IsProductionEnvironment()||src.IsPreProduction() ||src.IsDevelopmentEnv() || src.IsDevelopmentDemoEnv() || src.IsTest();
		}

		public static bool IsOneOfTheReleaseEnvironments(this IHostingEnvironment src)
		{
			return src.IsProduction()||src.IsPreProduction();
		}

		public static bool IsInternalApp(this IHostingEnvironment src)
		{
			return src.IsEnvironment(AppEnvironment.ProductionInternal)
			       || src.IsEnvironment(AppEnvironment.PreProdInternal)
			       || src.IsEnvironment(AppEnvironment.DevelopmentDemo)
			       || src.IsEnvironment(AppEnvironment.DevelopmentInternal)
			       || src.IsEnvironment(AppEnvironment.TestInternal);
		}
	}
}