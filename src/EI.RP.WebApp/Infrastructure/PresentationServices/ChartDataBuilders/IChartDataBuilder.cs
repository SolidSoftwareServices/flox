using System.Threading.Tasks;
using EI.RP.WebApp.Models.Charts;

namespace EI.RP.WebApp.Infrastructure.PresentationServices.ChartDataBuilders
{
	public interface IChartDataBuilder
	{
		PeriodType ForPeriodType { get; }
		Task<ConsumptionData> GetChartConsumptionData(UsageChartRequest request);
	}
}