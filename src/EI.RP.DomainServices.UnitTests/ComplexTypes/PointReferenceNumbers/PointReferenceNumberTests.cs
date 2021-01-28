using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.ComplexTypes.PointReferenceNumbers
{
	[TestFixture]
	public abstract class PointReferenceNumberTests
	{
		[Test]
		public abstract string ItCanCreateNewFromString(string input);

		[Test]
		public abstract string ItCanCreateNewFromNumber(int input);

		[Test]
		public abstract void WhenCreatingFromStringThrowsWhenNotValid(string input);

		[Test]
		public abstract void WhenCreatingFromNumberThrowsWhenNotValid(int input);

		[Test]
		public abstract string ItCanCastFromString(string input);

		[Test]
		public abstract string ItCanCastFromNumber(long input);

		[Test]
		public abstract void ItCanCastToString();

		[Test]
		public abstract void TypeIsCorrectlySet();

		[Test]
		public abstract bool ItCanBeCompared(
			Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers.PointReferenceNumber a,
			Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers.PointReferenceNumber b);
	}
}
