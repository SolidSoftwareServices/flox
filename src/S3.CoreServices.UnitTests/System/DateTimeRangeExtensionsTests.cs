using System;
using System.Collections.Generic;
using S3.CoreServices.System;
using NUnit.Framework;

namespace S3.CoreServices.UnitTests.System
{
	[TestFixture]
	class DateTimeRangeExtensionsTests
	{
		public static IEnumerable<TestCaseData> CanMergeConsecutivePeriodsCases()
		{
			yield return BuildTestCaseData(new DateTimeRange[0], "Empty");
			yield return BuildTestCaseData(new[] { Create(1, 2)}, "single");
			yield return BuildTestCaseData(new[] {Create(1, 2), Create(4, 5)}, "Not overlapped 1" );
			yield return BuildTestCaseDataForChange(new[] { Create(4, 5), Create(1, 2) }, new[] { Create(1, 2), Create(4, 5) }, "Not overlapped 2");
			yield return BuildTestCaseDataForChange(new[] {Create(1, 2), Create(3, 4)}, new[] {Create(1, 4)}, "Already consecutive");

			yield return BuildTestCaseDataForChange(new[] { Create(1, 3), Create(2, 4), Create(1, 3) }, new[] { Create(1, 4) }, "Repeated 1");
			yield return BuildTestCaseDataForChange(new[] { Create(1, 3), Create(2, 4), Create(1, 3), Create(2, 4) }, new[] { Create(1, 4) }, "Repeated 2");
			yield return BuildTestCaseDataForChange(new[] { Create(1, 3), Create(2, 4),  Create(2, 4), Create(1, 3) }, new[] { Create(1, 4) }, "Repeated 3");

			yield return BuildTestCaseDataForChange(new[] { Create(1, 3), Create(2, 4) }, new[] { Create(1, 4) }, "Overlapped 1");
			yield return BuildTestCaseDataForChange(new[] { Create(2, 4), Create(1, 3) }, new[] { Create(1, 4) }, "Overlapped 2");

			yield return BuildTestCaseDataForChange(new[] { Create(1, 4), Create(2, 3) }, new[] { Create(1, 4) }, "Fully Overlapped 1");
			yield return BuildTestCaseDataForChange(new[] { Create(2, 3), Create(1, 4) }, new[] { Create(1, 4) }, "Fully Overlapped 2");

			yield return BuildTestCaseDataForChange(new[] { Create(1, 2), Create(2,3), Create(3, 4), }, new[] { Create(1, 4) }, "Multiple 0");
			yield return BuildTestCaseDataForChange(new[] { Create(1, 2), Create(2, 3), Create(3, 4), Create(7, 8) }, new[] { Create(1, 4), Create(7, 8) }, "Multiple 1");
			yield return BuildTestCaseDataForChange(new[] { Create(1, 2), Create(2, 3), Create(7, 8),Create(3, 4) }, new[] { Create(1, 4), Create(7, 8) }, "Multiple 2");
			yield return BuildTestCaseDataForChange(new[] { Create(1, 2), Create(7, 8), Create(3, 4) }, new[] { Create(1, 4), Create(7, 8) }, "Multiple 3");
			yield return BuildTestCaseDataForChange(new[] { Create(7, 8),  Create(3, 4), Create(1, 2) }, new[] { Create(1, 4), Create(7, 8) }, "Multiple 4");



			DateTimeRange Create(int fromDays, int toDays)
			{
				return new DateTimeRange(DateTime.Today.AddDays(fromDays), DateTime.Today.AddDays(toDays));
			}

			TestCaseData BuildTestCaseData(IEnumerable<DateTimeRange> @in, string n)
			{
				return new TestCaseData(@in).SetName(n).Returns(@in);
			}

			TestCaseData BuildTestCaseDataForChange(IEnumerable<DateTimeRange> @in, IEnumerable<DateTimeRange> expected,
				string n)
			{
				return new TestCaseData(@in).SetName(n).Returns(expected);
			}
		}

		[TestCaseSource(nameof(CanMergeConsecutivePeriodsCases))]
		public IEnumerable<DateTimeRange> CanMergeConsecutivePeriods(IEnumerable<DateTimeRange> src)
		{
			return src.MergeConsecutivePeriods();
		}
	}
}