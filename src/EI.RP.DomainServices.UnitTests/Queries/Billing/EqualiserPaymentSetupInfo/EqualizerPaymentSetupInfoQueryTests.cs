using System;
using EI.RP.DataServices;
using EI.RP.DomainServices.Queries.Billing.EqualiserPaymentSetupInfo;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Queries.Billing.EqualiserPaymentSetupInfo
{
	[TestFixture]
	public class EqualizerPaymentSetupInfoQueryTests
	{
		[Test]
		public void FirstDueDateIsSetCorrectly([Range(1, 31)] int dayOfTheMonth)
		{

			var expected = new DateTime(2010, 01, dayOfTheMonth);
			var sut = new EqualizerPaymentSetupInfoQuery
			{
				FirstPaymentDateTime = expected
			};
			if (dayOfTheMonth > 28)
			{
				expected = new DateTime(2010, 02, 01);
			}

			Assert.AreEqual(expected, sut.FirstPaymentDateTime);

		}

		[Test]
		public void FirstDueDateIsInitializedCorrectly()
		{
			var fixDate = (Func<DateTime, DateTime>) (date =>
				date.Day > 28 ? new DateTime(date.Year, date.Month, 1).AddMonths(1) : date);
			var sut = new EqualizerPaymentSetupInfoQuery();

			var expected = fixDate(DateTime.Today.AddDays(10).Date);

			Assert.AreEqual(expected,sut.FirstPaymentDateTime);
		}
	}
}