using System;
using NLog;
using NUnit.Framework;
using S3.UI.TestServices.Sut;

namespace S3.UI.TestServices.Test
{
	[TestFixture,Parallelizable(ParallelScope.Fixtures)]
	public abstract class WebAppTestsBase<TApp> where TApp : ISutApp
	{

		protected readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly Func<TApp> _appFactoryFunc;

		protected WebAppTestsBase(Func<TApp> appFactoryFunc)
		{
			_appFactoryFunc = appFactoryFunc;
		}

		
		[SetUp]
		public virtual void SetUp()
		{
			App =_appFactoryFunc();
		}

		public TApp App { get; set; }
		
		[TearDown]
		public virtual void TearDown()
		{
			App.Release();
		}
	}
}