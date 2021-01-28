using System;
using System.Linq.Expressions;

namespace EI.RP.CoreServices.System.Lambda
{
	public static class LambdaComparerExtensions
	{
		public static bool IsEqualThan<TSource, TValue>(this Expression<Func<TSource, TValue>> x,Expression<Func<TSource, TValue>> y)
		{
			return LambdaComparer.Eq(x, y);
		}

		public static bool IsEqualThan<TSource1, TSource2, TValue>(
			Expression<Func<TSource1, TSource2, TValue>> x,
			Expression<Func<TSource1, TSource2, TValue>> y)
		{
			return LambdaComparer.Eq(x, y);
		}
	}
}