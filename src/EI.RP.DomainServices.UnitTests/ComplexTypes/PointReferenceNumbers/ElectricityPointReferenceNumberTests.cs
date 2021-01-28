using System;
using System.Collections.Generic;
using AutoFixture;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.DomainModels.SpecimenBuilders;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.ComplexTypes.PointReferenceNumbers
{
    class ElectricityPointReferenceNumberTests : PointReferenceNumberTests
    {

        [Test]
        public void ItCanBeCreatedUsingAutoFixture()
        {
            var actual = new Fixture()
                .CustomizeDomainTypeBuilders().Create<ElectricityPointReferenceNumber>();
            Assert.IsNotNull(actual);
        }
        public static IEnumerable<TestCaseData> ItCanCreateNewFromStringCases()
        {
            yield return new TestCaseData(null).Returns(null);
            yield return new TestCaseData(string.Empty).Returns(string.Empty);
            yield return new TestCaseData("754812").Returns("754812");
            yield return new TestCaseData("054812").Returns("054812");
            yield return new TestCaseData("10002193572").Returns("10002193572");
        }
        [TestCaseSource(nameof(ItCanCreateNewFromStringCases))]
        public override string ItCanCreateNewFromString(string input)
        {
            var sut = new ElectricityPointReferenceNumber(input);
            return sut.ToString();
        }
        [Ignore("Not supported")]
        public override string ItCanCreateNewFromNumber(int input)
        {
            throw new NotSupportedException();
        }
        public static IEnumerable<TestCaseData> WhenCreatingFromStringThrowsWhenNotValidCases()
        {
            yield return new TestCaseData(null);
            yield return new TestCaseData("1");
            yield return new TestCaseData("11002193572");
            yield return new TestCaseData("48125454");
            yield return new TestCaseData("21a481");
            yield return new TestCaseData("a");

        }
        [TestCaseSource(nameof(WhenCreatingFromStringThrowsWhenNotValidCases))]
        public override void WhenCreatingFromStringThrowsWhenNotValid(string input)
        {
            TestDelegate payload = () => new ElectricityPointReferenceNumber(input);
            if (input == null)
            {
                Assert.DoesNotThrow(payload);
            }
            else
            {
                Assert.Throws<ArgumentException>(payload);
            }
        }
        [Ignore("Not supported")]
        public override void WhenCreatingFromNumberThrowsWhenNotValid(int input)
        {
            throw new NotSupportedException();
        }

        public static IEnumerable<TestCaseData> ItCanCastFromStringCases()
        {
            yield return new TestCaseData("10002193572").Returns("10002193572");
            yield return new TestCaseData("054812").Returns("054812");
        }
        [TestCaseSource(nameof(ItCanCastFromStringCases))]
        public override string ItCanCastFromString(string input)
        {
            ElectricityPointReferenceNumber sut = input;
            return sut.ToString();
        }
        [Ignore("Not supported")]
        public override string ItCanCastFromNumber(long input)
        {
            throw new NotSupportedException();
        }

        [Test]
        public override void ItCanCastToString()
        {
            var expected = "10002193572";
            var actual = (string)new ElectricityPointReferenceNumber(expected);

            Assert.AreEqual(expected, actual);
        }
        [Test]
        public override void TypeIsCorrectlySet()
        {
            var sut = new ElectricityPointReferenceNumber("10002193572");
            Assert.AreEqual(PointReferenceNumberType.Mprn, sut.Type);
        }
        //case 01==0001
        public static IEnumerable<TestCaseData> ItCanBeComparedCases()
        {
            yield return new TestCaseData((ElectricityPointReferenceNumber)"10002193572", (ElectricityPointReferenceNumber)"10002193572").Returns(true);
            yield return new TestCaseData((ElectricityPointReferenceNumber)"054812", (ElectricityPointReferenceNumber)"054812").Returns(true);
            yield return new TestCaseData((ElectricityPointReferenceNumber)"054812", (GasPointReferenceNumber)"054812").Returns(false);
        }
        [TestCaseSource(nameof(ItCanBeComparedCases))]
        public override bool ItCanBeCompared(PointReferenceNumber a, PointReferenceNumber b)
        {
            return a == b;
        }
    }
}