using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EI.RP.CoreServices.DeliveryPipeline.Environments;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using EI.RP.CoreServices.System;
using Ei.Rp.Web.DebugTools;
using EI.RP.WebApp.Infrastructure.PresentationServices.ChartDataBuilders;
using EI.RP.WebApp.Models.Charts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace EI.RP.WebApp.Controllers
{
	public class ChartsController : Ei.Rp.Mvc.Core.Controllers.ControllerBase
	{
		private readonly IEnumerable<IChartDataBuilder> _builders;
		private readonly IConfigurableTestingItems _testingItems;
		private readonly IHostingEnvironment _hostingEnvironment;

		public ChartsController(IEnumerable<IChartDataBuilder> builders,IConfigurableTestingItems testingItems,IHostingEnvironment hostingEnvironment)
		{
			_builders = builders;
			_testingItems = testingItems;
			_hostingEnvironment = hostingEnvironment;
		}

		[HttpGet]
		public async Task<IActionResult> GetUsageChartData(UsageChartRequest request)
		{
			if (!_hostingEnvironment.IsOneOfTheReleaseEnvironments() &&_testingItems.SimulateConsumptionDataFailure)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}

			var period = request.Period.ToEnum<PeriodType>();

			var builder = _builders.Single(x => x.ForPeriodType == period);
			var chartData = await builder.GetChartConsumptionData(request);
			return new JsonResult(chartData);
		}
	}
}
