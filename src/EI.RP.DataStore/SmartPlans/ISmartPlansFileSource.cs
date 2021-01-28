using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EI.RP.DataModels.ResidentialPortal;

namespace EI.RP.DataStore.SmartPlans
{
	interface ISmartPlansFileSource
	{
		Task<IEnumerable<SmartActivationPlanDataModel>> ReadFileData();
	}
}
