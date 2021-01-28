using System.Threading.Tasks;
using Ei.Rp.DomainModels.Metering.Consumption;

namespace EI.RP.WebApp.Infrastructure.PresentationServices.FileBuilders.HDF
{
	public interface IHDFFileBuilder
	{
		Task<byte[]> BuildFileData(AccountConsumption consumption);
	}
}
