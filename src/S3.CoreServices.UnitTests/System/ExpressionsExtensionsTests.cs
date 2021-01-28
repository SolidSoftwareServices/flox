using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using S3.CoreServices.System;
using NUnit.Framework;

namespace S3.CoreServices.UnitTests.System
{
	[TestFixture]
	class ExpressionsExtensionsTests
	{
		public class A
		{
			public B BSingle { get; }
			public IEnumerable<B> BCollection { get; }
			public string AValue { get; }
		}

		public class B
		{
			public A ASingle { get; }
			public string BValue { get; }
		}
		public static IEnumerable<TestCaseData> GetExpressionPropertiesCases()
		{
			yield return new TestCaseData((Expression<Func<A, bool>>)(x =>1==2))
				.Returns($"");
			yield return  new TestCaseData((Expression<Func<A, bool>>)(x => x.BSingle.BValue == "s" && (x.AValue == "sss" || x.AValue == "ddd")))
				.Returns($"{nameof(A.AValue)},{nameof(A.BSingle)},{nameof(B.BValue)}");
		}
		[TestCaseSource(nameof(GetExpressionPropertiesCases))]
		[Test]
		public string GetExpressionProperties(Expression<Func<A, bool>> expr)
		{
			return string.Join(",",expr.GetExpressionProperties().Select(x=>x.Name));

		}
	}
}
