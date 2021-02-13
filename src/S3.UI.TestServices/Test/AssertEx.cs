using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace S3.UI.TestServices.Test
{
	public static class AssertEx
	{
		public static T Single<T>(IEnumerable<T> collection, Func<T,bool> predicate)
		{
			if (collection?.Count() != 1)
			{
				Assert.Fail("number of items is different than 0");
			}
			return collection.Single(predicate);
		}
	}
}