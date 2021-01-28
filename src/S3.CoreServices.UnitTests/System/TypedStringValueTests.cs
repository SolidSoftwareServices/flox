using System;
using S3.CoreServices.System;
using Newtonsoft.Json;
using NUnit.Framework;

namespace S3.CoreServices.UnitTests.System
{
	[TestFixture]
	class TypedStringValueTests
	{
		
		[Test]
		public void WhenValueIsDeclared_AndItsRequiredToExist_ItCanMapFromString()
		{
			TypedA actual = (TypedA) "Value";
			Assert.AreSame(TypedA.A,actual);

		}

		[Test]
		public void WhenValueIsNotDeclared_AndItsRequiredToExist_ItThrows()
		{
			Assert.Throws<InvalidCastException>(() =>
			{
				var actual = (TypedA) "NotValue";
			});
		}
		[Test]
		public void WhenValueIsDeclared__AndItsNotRequiredToExist_ItCanMapFromString()
		{
			var actual = (TypedB)"Value";
			Assert.AreSame(TypedB.A, actual);

		}
		[Test]
		public void WhenValueIsDeclared_AndItsNotRequiredToExist_ItCanMapTo_MappingNotConfigured()
		{
			var actual = (TypedB)"NotValue";
			Assert.IsNotNull(actual);
			Assert.AreEqual("NotValue",actual.ToString());
		}

		[Test]
		public void ItCanMapToString()
		{
			Assert.AreEqual("Value", TypedA.A.ToString());
		}

		class TypedA : TypedStringValue<TypedA>
		{
			[JsonConstructor]
			private TypedA()
			{
			}
			private TypedA(string value) : base(value)
			{
			}

			public static readonly TypedA A = new TypedA("Value");
		}

		class TypedB : TypedStringValue<TypedB>
		{
			[JsonConstructor]
			private TypedB()
			{
			}
			private TypedB(string value) : base(value,disableVerifyValueExists:true)
			{
			}

			public static readonly TypedB A = new TypedB("Value");
		}
	}
}