using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Infrastructure.PresentationServices.ChartDataBuilders;
using EI.RP.WebApp.Models.Charts;
using NLog;

namespace EI.RP.WebApp.Flows.AppFlows.Usage.Components.UsageChart
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
        private readonly IDomainQueryResolver _domainQueryResolver;
        private readonly IEnumerable<IChartDataBuilder> _builders;
        public ViewModelBuilder(IDomainQueryResolver domainQueryResolver, IEnumerable<IChartDataBuilder> builders)
        {
	        _domainQueryResolver = domainQueryResolver;
	        _builders = builders;
        }
	
		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
        {
            var accountInfo = await _domainQueryResolver.GetAccountInfoByAccountNumber(componentInput.AccountNumber);

            var smartPeriods = accountInfo.SmartPeriods.Where(x => x.Intersects(new DateTimeRange(DateTime.Today.AddYears(-2), DateTime.Today))).ToArray();
            var nonSmartPeriods = accountInfo.NonSmartPeriods.Where(x => x.Intersects(new DateTimeRange(DateTime.Today.AddYears(-2), DateTime.Today))).ToArray();
            
            var hasSmartPeriods = smartPeriods.Any();
            var hasNonSmartPeriods = nonSmartPeriods.Any();
			
            var minDate = new[]
            {
                hasSmartPeriods ? smartPeriods?.Min(x => x.Start) : null, 
                hasNonSmartPeriods ? nonSmartPeriods?.Min(x => x.Start) : null
            }.Min(x => x);
            
            var maxDate = new[]
            {
                hasSmartPeriods ? smartPeriods?.Max(x => x.End) : null,
                hasNonSmartPeriods ? nonSmartPeriods?.Max(x => x.End) : null
            }.Max(x => x);

            if (minDate == null || minDate < DateTime.Today.AddYears(-2))
            {
                minDate = DateTime.Today.AddYears(-2);
            }

            if (maxDate == null || maxDate > DateTime.Today)
            {
                maxDate = DateTime.Today;
            }

			minDate=  await AdjustMinDateToPeriodWithDataWhenNonSmart(componentInput, hasNonSmartPeriods,minDate.Value);

            var viewModel = new ViewModel
            {
                AccountNumber = componentInput.AccountNumber,
                CanCompare = componentInput.CanCompare,
                MinDate = minDate.Value,
                MaxDate = maxDate.Value,
                IsSmart = smartPeriods.Any(x => x.Contains(DateTime.Today)),
                HasSmartData = smartPeriods.Any(),
                HasNonSmartData = nonSmartPeriods.Any(),
                StartDatesOfSmartPlan = GetStartDatesOfSmartPlan(accountInfo),
                EndDatesOfSmartPlan = GetEndDatesOfSmartPlan(accountInfo),
                StartDatesOfNonSmartPlan = GetStartDatesOfNonSmartPlan(accountInfo)
            };

            return viewModel;
        }

		private async Task<DateTime> AdjustMinDateToPeriodWithDataWhenNonSmart(InputModel componentInput, bool hasNonSmartPeriods,DateTime minDate)
		{
			if (hasNonSmartPeriods && minDate < DateTime.Today.FirstDayOfTheYear())
			{

				int minYear;
				for (minYear = minDate.Year; minYear < DateTime.Today.Year; minYear++)
				{
					var builder = _builders.Single(x => x.ForPeriodType == PeriodType.Bimonthly);

					var data = await builder.GetChartConsumptionData(new UsageChartRequest
					{
						AccountNumber = componentInput.AccountNumber,
						Date = new[] {new DateTime(minYear, 1, 1), DateTime.Today.AddYears(-2)}.Max(),
						Period = PeriodType.Bimonthly.ToString()
					});

					if (data.Price.Values.Any(x => x != 0M) && data.Usage.Values.Any(x => x != 0M))
					{
						break;
					}
				}

				if (minYear > minDate.Year)
				{
					minDate = new DateTime(minYear, 1, 1);
				}
			}

			return minDate;
		}

		public static IEnumerable<string> GetStartDatesOfSmartPlan(AccountInfo account)
        {
            var list = account
                .SmartPeriods.Select(x => x.Start)
                .OrderBy(x => x)
                .Select(x => x.ToString("yyyy-MM-dd"))
                .ToList();

            return list;
        }

        public static IEnumerable<string> GetEndDatesOfSmartPlan(AccountInfo account)
        {
            var list = account
                .SmartPeriods.Select(x => x.End)
                .OrderBy(x => x)
                .Select(x => x.ToString("yyyy-MM-dd"))
                .ToList();

            return list;
        }

        public static IEnumerable<string> GetStartDatesOfNonSmartPlan(AccountInfo account)
        {
            var list = account
                .NonSmartPeriods.Select(x => x.Start)
                .OrderBy(x => x)
                .Select(x => x.ToString("yyyy-MM-dd"))
                .ToList();

            return list;
        }
    }
}
