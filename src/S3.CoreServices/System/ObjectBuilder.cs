using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace S3.CoreServices.System
{
	public class ObjectBuilder
	{
		public static readonly ObjectBuilder Default = new ObjectBuilder();

		private ObjectBuilder()
		{ }

		private readonly ConcurrentDictionary<string, Func<object>> _constructorCache =
			new ConcurrentDictionary<string, Func<object>>();

		public TResult Build<TResult>() //where TResult : new()
		{
			return (TResult) Build(typeof(TResult));
		}

		public object Build(Type type)
		{
			return _constructorCache.GetOrAdd(type.FullName.ToLower(), (t) =>
			{
				var ex = new Expression[] {Expression.New(type)};
				var block = Expression.Block(type, ex);
				return Expression.Lambda<Func<object>>(block).Compile();
			})();
		}


	}
}