using System;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Billing.InvoiceFiles;
using EI.RP.DomainServices.Queries.Metering.Consumption;
using Ei.Rp.Mvc.Core.Controllers;
using EI.RP.WebApp.Infrastructure.PresentationServices.FileBuilders.HDF;
using EI.RP.WebApp.Models.Files;
using Microsoft.AspNetCore.Mvc;

namespace EI.RP.WebApp.Controllers
{
	public class FilesController : Ei.Rp.Mvc.Core.Controllers.ControllerBase
	{
		private readonly IConfigurableTestingItems _testingItems;
		private readonly IDomainQueryResolver _domainQueryResolver;
		private readonly IHDFFileBuilder _hdfFileBuilder;

		public FilesController(IConfigurableTestingItems testingItems, IDomainQueryResolver domainQueryResolver,
			IHDFFileBuilder hdfFileBuilder)
		{
			_testingItems = testingItems;
			_domainQueryResolver = domainQueryResolver;
			_hdfFileBuilder = hdfFileBuilder;
		}

		[HttpGet]
		public async Task<IActionResult> GetInvoicePdf(InvoiceRequest request)
		{
			return await this.HandleActionAsync(async () =>
			{

				var file = await _domainQueryResolver.GetInvoiceFile(request.AccountNumber, request.ReferenceDocNumber);
				if (file == null)
				{
					return NotFound();
				}

				Response.Headers.Add("Content-Disposition", new ContentDisposition
				{
					FileName = file.GetFileName(),
					Inline = false,
					Size = file.FileData.Length
				}.ToString());

				Response.Headers.Add("Set-Cookie", "fileDownload=true; path=/");

				return File(file.FileData, "application/pdf");
			});
		}

		[HttpGet]
		public async Task<IActionResult> GetConsumptionDataFile(ConsumptionDataRequest request)
		{
			
			return await this.HandleActionAsync(async () =>
			{
				//you might need to implement custom error handling, see HandleActionAsync parameter
				if (_testingItems.SimulateConsumptionDataFailure)
				{
					return StatusCode((int)HttpStatusCode.InternalServerError);
				}
				

				var consumptionData = await _domainQueryResolver.GetAccountConsumption(request.AccountNumber,
					TimePeriodAggregationType.HalfHourly, new DateTimeRange(
						DateTime.Today.AddYears(-2),
						DateTime.Today), ConsumptionDataRetrievalType.Smart, fillResultWithZeroes: false);

				var fileData = await _hdfFileBuilder.BuildFileData(consumptionData);
				if (fileData == null || !fileData.Any())
				{
					return NotFound();
				}

				Response.Headers.Add("Content-Disposition", new ContentDisposition
				{
					FileName = $"consumption-{request.AccountNumber}-{DateTime.Now:ddMMyyyy-hhmm}.csv",
					Inline = false,
					Size = fileData.Length
				}.ToString());

				Response.Headers.Add("Set-Cookie", "fileDownload=true; path=/");

				return File(fileData, "text/csv");
			});
		}
	}
}