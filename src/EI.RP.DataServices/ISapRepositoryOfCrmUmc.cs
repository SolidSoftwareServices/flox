using System.Threading.Tasks;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap;


namespace EI.RP.DataServices
{
	public interface ISapRepositoryOfCrmUmc : IODataRepository, IDataService
	{
	}
}