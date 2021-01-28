using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.WebApp.Infrastructure.HealthChecks;
using EI.RP.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ControllerBase = Ei.Rp.Mvc.Core.Controllers.ControllerBase;

namespace EI.RP.WebApp.Controllers
{
	
	public class HealthStatusController : ControllerBase
	{
		private readonly IEnumerable<IResidentialPortalHealthCheck> _checkers;

		public HealthStatusController(IEnumerable<IResidentialPortalHealthCheck> checkers)
		{
			_checkers = checkers;
		}
		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2)))
			{
				var checkTasks = _checkers
					.Select(x => x.CheckHealthAsync(null, cts.Token)).ToArray();

				var results = await Task.WhenAll(checkTasks);
				var result = results.Min(x => x.Status );



				var defaultVersion = new Version(1, 0, 0, 0);

				var version = Assembly.GetEntryAssembly()?.GetName().Version ?? defaultVersion;
				var json = new JObject(
					new JProperty("version",
						version == defaultVersion ? "The version has not been set by the build" : version.ToString()),
					new JProperty("status", result.ToString()),
					new JProperty("results", results.Select(pair => new JObject(
						new JProperty("description", pair.Description),
						new JProperty("status", pair.Status.ToString()),
						new JProperty("exception", pair.Exception?.Message??"None"),
						new JProperty("extrainfo",string.Join(Environment.NewLine,pair.Data.Select(x=>$"{x.Key}={x.Value}")))
					))));
				return Ok(json.ToString(Formatting.Indented));
			}
		}

		[AllowAnonymous]
		[HttpGet]
		public IActionResult Loading()
		{
			return View("Loading", new ErrorViewModel());
		}
	}
}