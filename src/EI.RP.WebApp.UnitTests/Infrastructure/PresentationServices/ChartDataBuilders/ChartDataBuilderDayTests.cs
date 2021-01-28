using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering.Consumption;
using EI.RP.CoreServices.System;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Queries.Metering.Consumption;
using EI.RP.TestServices;
using EI.RP.WebApp.Infrastructure.PresentationServices.ChartDataBuilders;
using EI.RP.WebApp.Models.Charts;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.UnitTests.Infrastructure.PresentationServices.ChartDataBuilders
{
	[TestFixture]
	internal class ChartDataBuilderDayTests : UnitTestFixture<ChartDataBuilderDayTests.TestContext, ChartDataBuilderDay>
	{
		public class TestContext : UnitTestContext<ChartDataBuilderDay>
		{
			private string _accountNumber = DomainFacade.ModelsBuilder.Create<long>().ToString();
			private string _period = PeriodType.Day.ToString();
			private DateTime _dateTime;
			private AccountConsumption _accountConsumption;

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

			public UsageChartRequest BuildUsageChartRequest()
			{
				return Fixture.Build<UsageChartRequest>()
					.With(x => x.AccountNumber, _accountNumber)
					.With(x => x.Period, _period)
					.With(x => x.Date, _dateTime)
					.Create();
			}

			protected override ChartDataBuilderDay BuildSut(AutoMocker autoMocker)
			{
				DomainFacade.SetUpMocker(autoMocker);

				if (_accountConsumption != null)
				{
					DomainFacade.QueryResolver
						.ExpectQuery(new AccountConsumptionQuery
						{
							AccountNumber = _accountNumber,
							AggregationType = TimePeriodAggregationType.Hourly,
							Range = new DateTimeRange(_dateTime, _dateTime.LastTimeOfTheDay()),
							FillResultWithZeroes = true,
							RetrievalType = ConsumptionDataRetrievalType.Smart
						},
						_accountConsumption.ToOneItemArray());
				}

				return base.BuildSut(autoMocker);
			}
		}

		private AccountConsumption GetAccountConsumption(int numberOfDataEntries)
		{
			return new AccountConsumption
			{
				CostEntries = Enumerable.Range(0, numberOfDataEntries).Select(c => new AccountConsumption.CostEntry
				{
					Value = new EuroMoney(c),
					Date = DateTime.Today.AddMinutes(c * 30),
				}),
				UsageEntries = Enumerable.Range(0, numberOfDataEntries).Select(u => new AccountConsumption.UsageEntry
				{
					Value = new EuroMoney(u),
					Date = DateTime.Today.AddMinutes(u * 30),
				})
			};
		}


		[Test]
		public async Task TestChartDataBuilderForDay()
		{
			var startDate = DateTime.Today;
			var request = Context
				.WithPeriod(PeriodType.Day)
				.WithDate(startDate)
				.BuildUsageChartRequest();
			var consumptionData = GetAccountConsumption(10);
			var data = await Context
				.WithAccountConsumption(consumptionData)
				.Sut
				.GetChartConsumptionData(request);

			CollectionAssert.AreEqual(consumptionData.UsageEntries.Select(x => x.Value), data.Usage.Values);
			CollectionAssert.AreEqual(consumptionData.CostEntries.Select(x => x.Value.Amount), data.Price.Values);
		}
	}
}
