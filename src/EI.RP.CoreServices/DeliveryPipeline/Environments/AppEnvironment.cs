using EI.RP.CoreServices.System;
using Newtonsoft.Json;
using NotImplementedException = System.NotImplementedException;

namespace EI.RP.CoreServices.DeliveryPipeline.Environments
{
	public class AppEnvironment : TypedStringValue<AppEnvironment>
	{
		[JsonConstructor]
		private AppEnvironment()
		{
		}
		public AppEnvironment(string value) : base(value)
		{
		}
		
		public static readonly AppEnvironment CorrectTest = new AppEnvironment(nameof(CorrectTest));
		public static readonly AppEnvironment Development = new AppEnvironment(nameof(Development));
		public static readonly AppEnvironment DevelopmentInternal = new AppEnvironment($"{nameof(Development)}-Internal");

		public static readonly AppEnvironment DevelopmentDemo = new AppEnvironment(nameof(DevelopmentDemo));
		public static readonly AppEnvironment DevelopmentDemoInternal = new AppEnvironment($"{nameof(DevelopmentDemo)}-Internal");
		public static readonly AppEnvironment Test = new AppEnvironment(nameof(Test));
		public static readonly AppEnvironment TestInternal = new AppEnvironment($"{nameof(Test)}-Internal");
		public static readonly AppEnvironment PreProd = new AppEnvironment(nameof(PreProd));
		public static readonly AppEnvironment PreProdInternal = new AppEnvironment($"{nameof(PreProd)}-Internal");
		public static readonly AppEnvironment Production = new AppEnvironment(nameof(Production));
		public static readonly AppEnvironment ProductionInternal = new AppEnvironment($"{nameof(Production)}-Internal");

		public bool IsPublic()
		{
			return this.IsOneOf( Development, DevelopmentDemo,Test, PreProd, Production);
		}
		public bool IsInternal()
		{
			return this.IsOneOf( DevelopmentInternal, DevelopmentDemoInternal,TestInternal, PreProdInternal, ProductionInternal);
		}

		public string ResolveStageName()
		{
			return this.ToString().Replace("-Internal", string.Empty);
		}
	}
}