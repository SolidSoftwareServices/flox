using System.Threading.Tasks;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap;


namespace EI.RP.DataServices
{
	public interface ISapRepositoryOfUserManagement: IODataRepository, IDataService
	{
		Task<SapSessionData> LoginUser(string userName, string password, bool clearExistingSessionIfAnyOnError=true);
	}
}