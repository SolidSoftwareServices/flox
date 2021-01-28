using System;
using System.Linq.Expressions;
using System.Reflection;
using S3.CoreServices.System.FastReflection;
using NUnit.Framework;

namespace S3.CoreServices.UnitTests
{
	[TestFixture]
	public class PropertyExtensionsTests
	{

		private class Dummy
		{
			public string StringProperty { get; set; }

			public Dummy Inner { get; set; }
		}

		[Test]
		public void CanGetPropertyExpression()
		{
			var propertyInfo = typeof(Dummy).GetProperty(nameof(Dummy.StringProperty),BindingFlags.Instance|BindingFlags.Public|BindingFlags.GetProperty|BindingFlags.SetProperty);
			var actual = propertyInfo.GetPropertyExpression();

			Assert.AreEqual(typeof(Func<Dummy,string>),actual.GetType());
		}

		[Test]
		public void CanGetPropertyPath()
		{

			var exp = (Expression<Func<Dummy, string>>) (x => x.Inner.Inner.StringProperty);


			var actual = exp.GetPropertyPath();
			Assert.AreEqual("Inner.Inner.StringProperty", actual);
		}


		[Test]
		public void CanGetPropertyValueFast_FromPropertyExpression()
		{
			var expected = "Test";
			var target=new Dummy
			{
				Inner = new Dummy
				{
					Inner=new Dummy
					{
						StringProperty = expected
					}
				}
			};

			var actual = target.GetPropertyValueFast(x => x.Inner.Inner.StringProperty);
			Assert.AreEqual(expected, actual);
		}
	}
}