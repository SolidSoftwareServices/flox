using System;
using System.Collections.Generic;
using AutoFixture;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.DomainModels.SpecimenBuilders;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.ComplexTypes.PointReferenceNumbers
{
    class GasPointReferenceNumberTests : PointReferenceNumberTests
    {

        [Test]
        public void ItCanBeCreatedUsingAutoFixture()
        {
            var actual = new Fixture()
                .CustomizeDomainTypeBuilders().Create<GasPointReferenceNumber>();
            Assert.IsNotNull(actual);
        }
        public static IEnumerable<TestCaseData> ItCanCreateNewFromStringCases()
        {
            yield return new TestCaseData(null).Returns(null);
            yield return new TestCaseData(string.Empty).Returns(string.Empty);
            yield return new TestCaseData("2754812").Returns("2754812");
            yield return new TestCaseData("0754812").Returns("0754812");
            yield return new TestCaseData("15481").Returns("0015481");
            yield return new TestCaseData("1").Returns("0000001");
        }
        [TestCaseSource(nameof(ItCanCreateNewFromStringCases))]
        public override string ItCanCreateNewFromString(string input)
        {
            var sut = new GasPointReferenceNumber(input);
            return sut.ToString();
        }
        public static IEnumerable<TestCaseData> ItCanCreateNewFromNumberCases()
        {
            yield return new TestCaseData(1754812).Returns("1754812");
            yield return new TestCaseData(15481).Returns("0015481");
            yield return new TestCaseData(1).Returns("0000001");
        }
        [TestCaseSource(nameof(ItCanCreateNewFromNumberCases))]
        public override string ItCanCreateNewFromNumber(int input)
        {
            var sut = new GasPointReferenceNumber(input);
            return sut.ToString();
        }
        public static IEnumerable<TestCaseData> WhenCreatingFromStringThrowsWhenNotValidCases()
        {
            yield return new TestCaseData(null);
            yield return new TestCaseData("48125454");
            yield return new TestCaseData("21a481");
            yield return new TestCaseData("a");

        }
        [TestCaseSource(nameof(WhenCreatingFromStringThrowsWhenNotValidCases))]
        public override void WhenCreatingFromStringThrowsWhenNotValid(string input)
        {
            TestDelegate payload = () => new GasPointReferenceNumber(input);
            if (input == null)
            {
                Assert.DoesNotThrow(payload);
            }
            else
            {
                Assert.Throws<ArgumentException>(payload);
            }
        }
        public static IEnumerable<TestCaseData> WhenCreatingFromNumberThrowsWhenNotValidCases()
        {
            yield return new TestCaseData(-1);
            yield return new TestCaseData(0);
            yield return new TestCaseData(10000000);

        }
        [TestCaseSource(nameof(WhenCreatingFromNumberThrowsWhenNotValidCases))]
        public override void WhenCreatingFromNumberThrowsWhenNotValid(int input)
        {
            Assert.Throws<ArgumentException>(() => new GasPointReferenceNumber(input));
        }
        public static IEnumerable<TestCaseData> ItCanCastFromStringCases()
        {
            yield return new TestCaseData("2754812").Returns("2754812");
            yield return new TestCaseData("0754812").Returns("0754812");
            yield return new TestCaseData("15481").Returns("0015481");
            yield return new TestCaseData("1").Returns("0000001");
        }
        [TestCaseSource(nameof(ItCanCastFromStringCases))]
        public override string ItCanCastFromString(string input)
        {
            GasPointReferenceNumber sut = input;
            return sut.ToString();
        }
        public static IEnumerable<TestCaseData> ItCanCastFromNumberCases()
        {
            yield return new TestCaseData(1754812).Returns("1754812");
            yield return new TestCaseData(15481).Returns("0015481");
            yield return new TestCaseData(1).Returns("0000001");
        }
        [TestCaseSource(nameof(ItCanCastFromNumberCases))]
        public override string ItCanCastFromNumber(long input)
        {
            GasPointReferenceNumber sut = input;
            return sut.ToString();
        }

        [Test]
        public override void ItCanCastToString()
        {
            var expected = "2222222";
            var actual = (string)new GasPointReferenceNumber(expected);

            Assert.AreEqual(expected, actual);
        }
        [Test]
        public override void TypeIsCorrectlySet()
        {
            var sut = new GasPointReferenceNumber(1);
            Assert.AreEqual(PointReferenceNumberType.Gprn, sut.Type);
        }
        //case 01==0001
        public static IEnumerable<TestCaseData> ItCanBeComparedCases()
        {
            yield return new TestCaseData((GasPointReferenceNumber)"1", (GasPointReferenceNumber)"1").Returns(true);
            yield return new TestCaseData((GasPointReferenceNumber)"1", (GasPointReferenceNumber)"0001").Returns(true);
            yield return new TestCaseData((GasPointReferenceNumber)1, (GasPointReferenceNumber)2).Returns(false);
            yield return new TestCaseData((GasPointReferenceNumber)"111111", (ElectricityPointReferenceNumber)"111111").Returns(false);
        }
        [TestCaseSource(nameof(ItCanBeComparedCases))]
        public override bool ItCanBeCompared(PointReferenceNumber a, PointReferenceNumber b)
        {
            return a == b;
        }
    }
}