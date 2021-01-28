using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Queries.Metering.Consumption.Services;
using EI.RP.TestServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.UnitTests.Queries.Metering.Consumption.Services
{
	[TestFixture]
	internal class SmartConsumptionResolverTest : UnitTestFixture<SmartConsumptionResolverTest.TestContext, SmartConsumptionResolver>
	{
		internal class TestContext : UnitTestContext<SmartConsumptionResolver>
		{
		}

		internal class TestCase
		{
			public DateTimeRange DateTimeRange { get; set; }
			public int ChunkSize { get; set; }
			public List<DateTimeRange> ExpectedResultRanges { get; set; }
			public TimePeriodAggregationType AggregationType { get; set; }

			public override string ToString()
			{
				return $"AggregationType: {AggregationType} - {DateTimeRange}";
			}
		}

		private const int DaysInAYear = 365;

		public static IEnumerable<TestCaseData> CanGetCorrectDateRage()
		{
			var testCases = new[]
			{
			    GetTestCase(TimePeriodAggregationType.Daily, new DateTimeRange(DateTime.Today.AddDays(-20), DateTime.Today)),
				GetTestCase(TimePeriodAggregationType.Daily, new DateTimeRange(DateTime.Today.AddYears(-2), DateTime.Today)),
				GetTestCase(TimePeriodAggregationType.Monthly, new DateTimeRange(DateTime.Today.AddYears(-2), DateTime.Today)),
				GetTestCase(TimePeriodAggregationType.HalfHourly, new DateTimeRange(DateTime.Today.AddYears(-2), DateTime.Today)),
				GetTestCase(TimePeriodAggregationType.HalfHourly, new DateTimeRange(DateTime.Today.AddMonths(-2), DateTime.Today)),
			    GetTestCase(TimePeriodAggregationType.Hourly, new DateTimeRange(DateTime.Today.AddYears(-2), DateTime.Today)),
		    };

			foreach (var testCase in testCases)
			{
				yield return new TestCaseData(testCase.DateTimeRange, testCase.ChunkSize).SetName(testCase.ToString()).Returns(testCase.ExpectedResultRanges);
			}
		}

		private static TestCase GetTestCase(TimePeriodAggregationType aggregationType, DateTimeRange range)
		{
			var chunkSize = GetChunkSize(aggregationType);
			var start = range.Start.Date;
			var end = range.End.Date;
			var resultCount = Convert.ToInt32((end - start).TotalDays) / chunkSize + 1;

			return new TestCase
			{
				DateTimeRange = range,
				AggregationType = aggregationType,
				ChunkSize = chunkSize,
				ExpectedResultRanges = Enumerable
					.Range(0, resultCount)
					.Select(x => new DateTimeRange(start.AddDays(chunkSize * x),
						start.AddDays(chunkSize * (x + 1)).AddMilliseconds(-1) > end
							? end
							: start.AddDays(chunkSize * (x + 1)).AddMilliseconds(-1))).ToList()
			};
		}

		private static int GetChunkSize(TimePeriodAggregationType aggregationType)
		{
			if (aggregationType == TimePeriodAggregationType.Monthly)
			{
				return DaysInAYear * 2;
			}
			if (aggregationType == TimePeriodAggregationType.Daily)
			{
				return DaysInAYear;
			}
			if (aggregationType == TimePeriodAggregationType.Hourly ||
				aggregationType == TimePeriodAggregationType.HalfHourly)
			{
				return DaysInAYear / 4;
			}
			throw new NotSupportedException();
		}

		[TestCaseSource(nameof(CanGetCorrectDateRage))]
		public List<DateTimeRange> CreateSmartDataSplitRange(DateTimeRange smartDateTimeRanges, int dayChunkSize)
		{
			return Context.Sut.CreateSmartDataSplitRange(new[] { smartDateTimeRanges }, dayChunkSize);
		}


	}
}
