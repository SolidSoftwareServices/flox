using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace EI.RP.DataServices
{
	public interface IStreamServeRepository: IDataService
	{
	
		Task<StreamContent> GetInvoiceFileStream(string invoiceDocNumber, CancellationToken cancellationToken = default(CancellationToken));
		Task<bool> IsHealthy(CancellationToken cancellationToken = default(CancellationToken));
	}
}