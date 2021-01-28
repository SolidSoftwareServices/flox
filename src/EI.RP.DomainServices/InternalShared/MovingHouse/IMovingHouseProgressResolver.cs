using System.Threading.Tasks;
using EI.RP.DataModels.ResidentialPortal;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainServices.InternalShared.MovingHouse
{
	internal interface IMovingHouseProgressResolver
	{
		long GetUniqueId(AccountInfo accountInitiated, MovingHouseType commandDataMoveType);
		Task<AccountInfo> ResolveAccount(MovingHouseProcessStatusDataModel dataModel, MovingHouseType moveType);
	}
}
