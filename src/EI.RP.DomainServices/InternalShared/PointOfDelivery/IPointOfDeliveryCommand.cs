using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;

namespace EI.RP.DomainServices.InternalShared.PointOfDelivery
{
	internal interface IPointOfDeliveryCommand
	{
		Task<PointOfDeliveryInfo> AddPointOfDelivery(PointReferenceNumber prn, 
													ClientAccountType accountType,
												    string usePremisesAddressOfAccountNumber = null, 
													string usePremisesAddressFromPremiseId = null);
	}
}