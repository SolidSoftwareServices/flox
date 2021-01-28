using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace S3.TestServices
{

	/// <summary>
	/// Base class for all unit tests... From now on
	/// The contexts will run each tests in parallel with isolation
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	/// <typeparam name="TSut"></typeparam>
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public abstract class UnitTestFixture<TContext,TSut>
		where TSut : class
		where TContext:UnitTestContext<TSut>, new()
	{

		private readonly IDictionary<string, TContext> _contexts = new ConcurrentDictionary<string, TContext>();

		protected TContext Context =>_contexts[ResolveTestContextKey()];

		private static string ResolveTestContextKey()
		{
			return TestContext.CurrentContext.Test.FullName;
		}


		[SetUp]
		public virtual void SetUp()
		{
			_contexts.Add(ResolveTestContextKey(), new TContext());
		}

		[TearDown]
		public virtual void TearDown()
		{
			Context.Dispose();
			_contexts.Remove(ResolveTestContextKey());
		}
	}

}