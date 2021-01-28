using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Metering.Consumption;
using EI.RP.TestServices.Logging;
using NLog;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.Metering
{
	[Explicit("TODO:TEST")]
	[TestFixture]
	public class AccountConsumptionQueryTests : DomainTests
	{
		public static IEnumerable<TestCaseData> CanGetQuery()
		{
			foreach (var consumptionAggregationType in TimePeriodAggregationType.AllValues
				.Cast<TimePeriodAggregationType>().ToArray())
			{
				yield return new TestCaseData("HHMT2years1@test.ie", "Test3333",consumptionAggregationType);

				yield return new TestCaseData("24HRLT2years1@test.ie", "Test3333", consumptionAggregationType);

				yield return new TestCaseData("2yearsHHdata_set1@PP.ie", "Test3333", consumptionAggregationType);
				yield return new TestCaseData("2yearsHHdata_set2@PP.ie", "Test3333", consumptionAggregationType);

			}
		}

		[Test, TestCaseSource(nameof(CanGetQuery))]
		public async Task CanQuery(string userName, string password, TimePeriodAggregationType aggregationType)
		{
			TestLogging.Default.ConfigureLogging(null,LogLevel.Off);
			await LoginUser(userName, password);
			var accounts = await DomainQueryProvider.GetAccounts();

			foreach (var account in accounts)
			{
				var sw = new Stopwatch();
				sw.Start();
				var result = await DomainQueryProvider.GetAccountConsumption(account.AccountNumber, aggregationType,new DateTimeRange(DateTime.Today.FirstDayOfNextMonth().AddYears(-2),DateTime.Today.FirstDayOfNextMonth().AddYears(2)), 
					ConsumptionDataRetrievalType.Smart, fillResultWithZeroes: true);
				sw.Stop();
				Assert.IsNotNull(result);


				Assert.AreEqual(result.UsageEntries.Count(),result.CostEntries.Count());
				Console.WriteLine($"entries:{result.UsageEntries.Count()} -Total Days:{result.UsageEntries.Count()/ResolveToDays()} - Time: {sw.Elapsed} ");
			}
			TestLogging.Default.ConfigureLogging();
			float ResolveToDays()
			{
				if (aggregationType == TimePeriodAggregationType.HalfHourly)
				{
					return 48;
				}
				if (aggregationType == TimePeriodAggregationType.Hourly)
				{
					return 24;
				}
				if (aggregationType == TimePeriodAggregationType.Daily)
				{
					return 1;
				}
				return 1/30;

			}
		}
		public static IEnumerable<TestCaseData> CanQueryBimonthlyCases()
		{
			
			yield return new TestCaseData("24HRMT2years1@test.ie", "Test3333");
			
		}

		[Test, TestCaseSource(nameof(CanQueryBimonthlyCases))]
		public async Task CanQueryBimonthly(string userName, string password)
		{
			TestLogging.Default.ConfigureLogging(null, LogLevel.Off);
			await LoginUser(userName, password);
			var accounts = await DomainQueryProvider.GetAccounts();

			foreach (var account in accounts)
			{
				var sw = new Stopwatch();
				sw.Start();
				var result = await DomainQueryProvider.GetAccountConsumption(account.AccountNumber, TimePeriodAggregationType.BiMonthly, new DateTimeRange(DateTime.Today.AddYears(-2), DateTime.Today.AddYears(2)), ConsumptionDataRetrievalType.NonSmart, fillResultWithZeroes: true);
				sw.Stop();
				Assert.IsNotNull(result);
				Assert.AreEqual(result.UsageEntries.Count(), result.CostEntries.Count());
				Console.WriteLine($"entries:{result.UsageEntries.Count()}  - Time: {sw.Elapsed} ");
			}
			TestLogging.Default.ConfigureLogging();
			
		}

	}
}