using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.DataModels.ResidentialPortal;

namespace EI.RP.DataServices
{
	public interface IResidentialPortalDataRepository: IDataService
	{
		Task<CompetitionEntryDto> GetCompetitionEntry(string userName);

		Task<CompetitionEntryDto> AddCompetitionEntry(CompetitionEntryDto newItem);
		Task<MovingHouseProcessStatusDataModel> SetMovingHouseProcessStatus(MovingHouseProcessStatusDataModel data);

		Task<MovingHouseProcessStatusDataModel> GetMovingHouseProcessStatus(long identity);

		SmartActivationNotificationDto GetSmartActivationNotificationInfo(string userName,string accountNumber);
		SmartActivationNotificationDto Save(SmartActivationNotificationDto newItem);
		Task<IEnumerable<SmartActivationPlanDataModel>> GetSmartActivationPlans(string groupName);
	}
}