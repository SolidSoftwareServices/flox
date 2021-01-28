using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EI.RP.DataServices
{
	public interface IPdfOverlayImageRepository:IDataService
	{
		Task<StreamContent> GetImageStreamAsync(string fileName,CancellationToken cancellationToken=default(CancellationToken));
	}
}