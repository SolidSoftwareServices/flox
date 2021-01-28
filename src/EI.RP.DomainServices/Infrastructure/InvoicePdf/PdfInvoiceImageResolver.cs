using System;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DataServices;
using iText.IO.Image;
using NLog;

namespace EI.RP.DomainServices.Infrastructure.InvoicePdf
{
	public class PdfInvoiceImageResolver : IPdfInvoiceImageResolver
	{
		private readonly IPdfOverlayImageRepository _repository;

		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		public PdfInvoiceImageResolver(IPdfOverlayImageRepository repository)
		{
			_repository = repository;
		}

		public async Task<ImageData> ResolvePdfInvoicePagePicture(int pageNum, string language,
			ClientAccountType clientAccountType)
		{
			var fileName = ResolveFileName(pageNum, language, clientAccountType);
			var stream = await _repository.GetImageStreamAsync(fileName);
			return ImageDataFactory.Create(await stream.ReadAsByteArrayAsync());
		}


		private string ResolveFileName(int pageNum, string language, ClientAccountType clientAccountType)
		{
			string ResolveAccountTypeToken()
			{
				if (pageNum > 2 && clientAccountType == ClientAccountType.Electricity) return string.Empty;
				if (clientAccountType == ClientAccountType.Electricity) return "res_";
				if (clientAccountType == ClientAccountType.Gas) return "gas_";
				throw new NotSupportedException(clientAccountType);
			}

			string ResolveLanguageToken()
			{
				if (pageNum > 2) return string.Empty;
				return $"{language}_";
			}

			string ResolvePageToken()
			{
				if (pageNum > 2) return "continuation_generic_overlay";
				return $"p{pageNum}_overlay";
			}

			return
				$"{ResolveAccountTypeToken()}{ResolveLanguageToken()}{ResolvePageToken()}.jpg";
		}


	}
}
