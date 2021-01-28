using System;
using EI.RP.UI.TestServices.Sut;
using NLog;
using NUnit.Framework;

namespace EI.RP.UI.TestServices.Test
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