using AutoFixture;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering.Consumption;
using EI.RP.CoreServices.System;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Metering.Consumption;
using EI.RP.TestServices;
using EI.RP.WebApp.Infrastructure.PresentationServices.ChartDataBuilders;
using EI.RP.WebApp.Models.Charts;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.UnitTests.Infrastructure.PresentationServices.ChartDataBuilders
{
	[TestFixture]
	internal class ChartDataBuilderYearTests : UnitTestFixture<ChartDataBuilderYearTests.TestContext, ChartDataBuilderYear>
	{
		public class TestContext : UnitTestContext<ChartDataBuilderYear>
		{
			private string _accountNumber = DomainFacade.ModelsBuilder.Create<long>().ToString();
			private string _period = PeriodType.Year.ToString();
			private DateTime _dateTime;
			private AccountConsumption _accountConsumption;
			private AccountInfo _accountInfo;

			public static DomainFacade DomainFacade { get; } = new DomainFacade();

			public TestContext WithAccountNumber(string accountNumber)
			{
				_accountNumber = accountNumber;
				return this;
			}

			public TestContext WithPeriod(PeriodType period)
			{
				_period = period.ToString();
				return this;
			}

			public TestContext WithDate(DateTime dateTime)
			{
				_dateTime = dateTime;
				return this;
			}

			public TestContext WithAccountConsumption(AccountConsumption accountConsumption)
			{
				_accountConsumption = accountConsumption;
				return this;
			}

			public TestContext WithAccountInfo(AccountInfo accountInfo)
			{
				_accountInfo = accountInfo;
				return this;
			}

			public UsageChartRequest BuildUsageChartRequest()
			{
				return Fixture.Build<UsageChartRequest>()
					.With(x => x.AccountNumber, _accountNumber)
					.With(x => x.Period, _period)
					.With(x => x.Date, _dateTime)
					.Create();
			}

			protected override ChartDataBuilderYear BuildSut(AutoMocker autoMocker)
			{
				DomainFacade.SetUpMocker(autoMocker);

				if (_accountConsumption != null)
				{
					DomainFacade.QueryResolver
						.ExpectQuery(new AccountConsumptionQuery
						{
							AccountNumber = _accountNumber,
							AggregationType = TimePeriodAggregationType.Monthly,							
							Range = new DateTimeRange(_dateTime.FirstDayOfTheYear(), _dateTime.LastDayOfTheYear()),
							FillResultWithZeroes = true,
							RetrievalType = ConsumptionDataRetrievalType.Smart
						},
						_accountConsumption.ToOneItemArray());

					DomainFacade.QueryResolver
						.ExpectQuery(new AccountConsumptionQuery
						{
							AccountNumber = _accountNumber,
							AggregationType = TimePeriodAggregationType.BiMonthly,
							Range = new DateTimeRange(_dateTime.FirstDayOfTheYear(), _dateTime.LastDayOfTheYear()),
							FillResultWithZeroes = true,
							RetrievalType = ConsumptionDataRetrievalType.NonSmart
						},
						_accountConsumption.ToOneItemArray());
				}

				if (_accountNumber != null)
				{
					DomainFacade.QueryResolver
						.ExpectQuery(new AccountInfoQuery
						{
							AccountNumber = _accountNumber,							
						},
						_accountInfo.ToOneItemArray());
				}
				
				return base.BuildSut(autoMocker);
			}
			
		}

		private AccountConsumption GetAccountConsumption(int numberOfDataEntries, DateTime firstOfYear)
		{
			return new AccountConsumption
			{

			    CostEntries = Enumerable.Range(1, numberOfDataEntries).Select(c => new AccountConsumption.CostEntry
				{
					Value = new EuroMoney(c),
					Date = new DateTime(firstOfYear.AddMonths(c).Year, c*2, DateTime.DaysInMonth(firstOfYear.AddMonths(c).Year, c*2))
				}),
				UsageEntries = Enumerable.Range(1, numberOfDataEntries).Select(u => new AccountConsumption.UsageEntry
				{
					Value = new EuroMoney(u),
					Date = new DateTime(firstOfYear.AddMonths(u).Year, u * 2, DateTime.DaysInMonth(firstOfYear.AddMonths(u).Year, u * 2))
				})
			};
		}

		private AccountInfo GetAccountInfo(string accountNumber, DateTime firstDayOfYear, DateTime lastDayOfYear)
		{
			var nonSmartPeriod = new List<DateTimeRange>();
			var smartPeriods = new List<DateTimeRange>();
			smartPeriods.Add(new DateTimeRange(firstDayOfYear, firstDayOfYear.AddMonths(5)));
			nonSmartPeriod.Add(new DateTimeRange(firstDayOfYear.AddMonths(5).AddDays(1), lastDayOfYear));

			return new AccountInfo
			{
				AccountNumber = accountNumber,
				NonSmartPeriods = nonSmartPeriod,
				SmartPeriods = smartPeriods
			};
		}

		[Test]
		public async Task TestChartDataBuilderForYear()
		{
			var startDate = DateTime.Today.FirstDayOfTheYear();
			var request = Context
				.WithPeriod(PeriodType.Year)
				.WithDate(startDate)
				.BuildUsageChartRequest();
			var firstDayOfYear = request.Date.Date.FirstDayOfTheYear();
			var lastDayOfYear = request.Date.Date.LastDayOfTheYear();
			var consumptionData = GetAccountConsumption(6, firstDayOfYear);
			var accountInfo = GetAccountInfo(request.AccountNumber, firstDayOfYear, lastDayOfYear);
			var data = await Context
				.WithAccountConsumption(consumptionData)
				.WithAccountInfo(accountInfo)
				.Sut
				.GetChartConsumptionData(request);

			Assert.AreEqual(consumptionData.UsageEntries.Count(), data.Usage.Values.Count());
			Assert.AreEqual(consumptionData.CostEntries.Count(), data.Price.Values.Count());
		}

	}
}
