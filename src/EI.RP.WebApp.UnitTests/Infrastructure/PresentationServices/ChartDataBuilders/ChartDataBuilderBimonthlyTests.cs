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
	internal class ChartDataBuilderBiMonthlyTests : UnitTestFixture<ChartDataBuilderBiMonthlyTests.TestContext, ChartDataBuilderBimonthly>
	{
		public class TestContext : UnitTestContext<ChartDataBuilderBimonthly>
		{
			private string _accountNumber = DomainFacade.ModelsBuilder.Create<long>().ToString();
			private string _period = PeriodType.Year.ToString();
			private DateTime _startDate;
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

			public TestContext WithStartDate(DateTime dateTime)
			{
				_startDate = dateTime;
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
					.With(x => x.Date, _startDate)
					.Create();
			}

			protected override ChartDataBuilderBimonthly BuildSut(AutoMocker autoMocker)
			{
				DomainFacade.SetUpMocker(autoMocker);

				if (_accountConsumption != null)
				{
					DomainFacade.QueryResolver
						.ExpectQuery(new AccountConsumptionQuery
						{
							AccountNumber = _accountNumber,
							AggregationType = TimePeriodAggregationType.BiMonthly,
							Range = new DateTimeRange(_startDate.FirstDayOfTheYear(), _startDate.LastTimeOfTheYear()),
							FillResultWithZeroes = true,
							RetrievalType = ConsumptionDataRetrievalType.NonSmart
						},
						_accountConsumption.ToOneItemArray());

					DomainFacade.QueryResolver
						.ExpectQuery(new AccountConsumptionQuery
						{
							AccountNumber = _accountNumber,
							AggregationType = TimePeriodAggregationType.Monthly,
							Range = new DateTimeRange(_startDate.FirstDayOfTheYear(), _startDate.LastTimeOfTheYear()),
							FillResultWithZeroes = true,
							RetrievalType = ConsumptionDataRetrievalType.Smart
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
			var domainFacade = new DomainFacade();
			return new AccountConsumption
			{
				CostEntries = Enumerable.Range(1, numberOfDataEntries).Select(c => new AccountConsumption.CostEntry
				{
					Value = new EuroMoney(c),
					Date = firstOfYear.AddMonths(c),
					Prn = c > numberOfDataEntries / 2 ? domainFacade.ModelsBuilder.Create<string>() : string.Empty,
					SerialNumber = c > numberOfDataEntries / 2 ? domainFacade.ModelsBuilder.Create<string>() : string.Empty,
				}),
				UsageEntries = Enumerable.Range(1, numberOfDataEntries).Select(u => new AccountConsumption.UsageEntry
				{
					Value = new EuroMoney(u),
					Date = firstOfYear.AddMonths(u),
					Prn = u > numberOfDataEntries / 2 ? domainFacade.ModelsBuilder.Create<string>() : string.Empty,
					SerialNumber = u > numberOfDataEntries / 2 ? domainFacade.ModelsBuilder.Create<string>() : string.Empty,
				})
			};
			
		}

		private AccountInfo GetAccountInfo(string accountNumber, DateTime firstDayOfYear, DateTime lastDayOfYear)
		{
			var nonSmartPeriod = new List<DateTimeRange>
			{
				new DateTimeRange(firstDayOfYear, firstDayOfYear.AddMonths(5).AddDays(1))
			};

			return new AccountInfo
			{
				AccountNumber = accountNumber,
				NonSmartPeriods = nonSmartPeriod,
			};
		}

		[Test]
		public async Task TestChartDataBuilderForBimonthly()
		{
			var numberOfExpectedEntries = 6;
			var startDate = DateTime.Today.FirstDayOfTheYear();
			var request = Context
				.WithPeriod(PeriodType.Bimonthly)
				.WithStartDate(startDate)
				.BuildUsageChartRequest();
			var firstDayOfYear = request.Date.FirstDayOfTheYear();
			var lastTimeOfTheYear = request.Date.LastTimeOfTheYear();
			var consumptionData = GetAccountConsumption(numberOfExpectedEntries, firstDayOfYear);
			var accountInfo = GetAccountInfo(request.AccountNumber, firstDayOfYear, lastTimeOfTheYear);
			var data = await Context
				.WithAccountConsumption(consumptionData)
				.WithAccountInfo(accountInfo)
				.Sut
				.GetChartConsumptionData(request);
			int nonSmartUsageEntriesCount = 0;
			var nonSmartUsageEntries = consumptionData.UsageEntries.ToList().Where(x => string.IsNullOrEmpty(x.Prn) && string.IsNullOrWhiteSpace(x.SerialNumber))
				                       .Count();
			nonSmartUsageEntriesCount = nonSmartUsageEntries/2 + nonSmartUsageEntries % 2;
			int smartUsageEntriesCount = 0;
			var smartUsageEntries = consumptionData.UsageEntries.ToList().Where(x => !string.IsNullOrEmpty(x.Prn) && !string.IsNullOrWhiteSpace(x.SerialNumber))
									   .Count();
			smartUsageEntriesCount = smartUsageEntries / 2 + smartUsageEntries % 2;

			int nonSmartCostEntriesCount = 0;
			var nonSmartCostEntries = consumptionData.CostEntries.ToList().Where(x => string.IsNullOrEmpty(x.Prn) && string.IsNullOrWhiteSpace(x.SerialNumber))
									   .Count();
			nonSmartCostEntriesCount = nonSmartCostEntries / 2 + nonSmartCostEntries % 2;
			int smartCostEntriesCount = 0;
			var smartCostEntries = consumptionData.CostEntries.ToList().Where(x => !string.IsNullOrEmpty(x.Prn) && !string.IsNullOrWhiteSpace(x.SerialNumber))
									   .Count();
			smartCostEntriesCount = smartCostEntries / 2 + smartCostEntries % 2;

			Assert.IsTrue(data.Usage.Values.Count() == nonSmartUsageEntriesCount + smartUsageEntriesCount);
			Assert.IsTrue(data.Price.Values.Count() == nonSmartCostEntriesCount + smartCostEntriesCount);
		}
	}
}