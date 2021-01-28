using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using iText.IO.Image;

namespace EI.RP.DomainServices.Infrastructure.InvoicePdf
{
	public interface IPdfInvoiceImageResolver
	{
		Task<ImageData> ResolvePdfInvoicePagePicture(int pageNum, string language, ClientAccountType clientAccountType);
	}
}