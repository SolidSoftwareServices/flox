using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.OData.Client.Infrastructure.Validation;
using NUnit.Framework;

namespace EI.RP.CoreServices.OData.Client.UnitTests.Infrastructure.Validation
{
	[TestFixture]
	public class ProxyModelValidatorTests
	{
		private ProxyModelValidator _sut;

		[SetUp]
		public void Setup()
		{
			_sut = new ProxyModelValidator();
		}


		public static IEnumerable<TestCaseData> CanValidateRequiredCases()
		{
			yield return new TestCaseData(new TestModel(),true).Returns($"The {nameof(TestModel.NotNullable)} field is required.");
			yield return new TestCaseData(new TestModel { NotNullable = string.Empty }, true).Returns($"The {nameof(TestModel.NotNullable)} field is required.");
			yield return new TestCaseData(new TestModel{NotNullable ="a"}, false).Returns(null);
		}

		[TestCaseSource(nameof(CanValidateRequiredCases))]
		public string CanValidateRequired (TestModel model,bool throws)
		{
			TestDelegate validationDelegate = ()=>_sut.Validate(model,ProxyModelOperation.Update);
			string result=null;
			if (throws)
			{
				var actual = Assert.Throws<DomainException>(validationDelegate);
				result = ((ValidationException) actual.InnerException).Message;
			}
			else
			{
				Assert.DoesNotThrow(validationDelegate);
			}

			return result;
		}

		public static IEnumerable<TestCaseData> CanValidateMaxLengthCases()
		{
			yield return new TestCaseData(new TestModel { NotNullable = "a",MaxLength3 = null}, false).Returns(null);
			yield return new TestCaseData(new TestModel { NotNullable = "a", MaxLength3 = string.Empty }, false).Returns(null);
			yield return new TestCaseData(new TestModel { NotNullable = "a", MaxLength3 = "aaa"}, false).Returns(null);
			yield return new TestCaseData(new TestModel { NotNullable = "a", MaxLength3 = "aaaa" }, true).Returns($"The field {nameof(TestModel.MaxLength3)} must be a string with a maximum length of 3.");
		}

		[TestCaseSource(nameof(CanValidateMaxLengthCases))]
		public string CanValidateMaxLength(TestModel model, bool throws)
		{
			TestDelegate validationDelegate = () => _sut.Validate(model,ProxyModelOperation.Update);
			string result = null;
			if (throws)
			{
				var actual = Assert.Throws<DomainException>(validationDelegate);
				result = ((ValidationException)actual.InnerException).Message;
			}
			else
			{
				Assert.DoesNotThrow(validationDelegate);
			}

			return result;
		}

		public class TestModel
		{

			public string Nullable { get; set; }
			[Required]
			public string NotNullable { get; set; }

			[StringLength(3)]
			public string MaxLength3 { get; set; }

			public override string ToString()
			{
				return $"{nameof(Nullable)}: {Nullable}, {nameof(NotNullable)}: {NotNullable}";
			}
		}
	}
}