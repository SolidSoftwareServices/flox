using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;

namespace EI.RP.DomainServices.InternalShared.MeterReading
{
	internal interface ISubmitMeterReadings
	{
		Task SubmitGasMeterReading(string accountNumber, bool isNewAcquisition, GasPointReferenceNumber premisePrn,
			decimal meterReading, bool isAddGas = false);
	}
}