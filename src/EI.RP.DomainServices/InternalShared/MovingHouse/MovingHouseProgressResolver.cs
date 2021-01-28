using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.ResidentialPortal;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.InternalShared.MovingHouse
{
	class MovingHouseProgressResolver : IMovingHouseProgressResolver
	{

		private readonly IDomainQueryResolver _queryResolver;

		public MovingHouseProgressResolver(IDomainQueryResolver queryResolver)
		{
			_queryResolver = queryResolver;
		}


		public long GetUniqueId(AccountInfo accountInitiated, MovingHouseType moveType)
		{
			var str = $"{MovingHouseType.AllValues.IndexOf(moveType)}{accountInitiated.AccountNumber}";
			return long.Parse(str);
		}

		public async Task<AccountInfo> ResolveAccount(MovingHouseProcessStatusDataModel dataModel, MovingHouseType moveType)
		{
			if (dataModel == null) return null;

			var electricity = TryAccount(dataModel.ELEC_CONTRACT_ACCOUNT);
			var gas = TryAccount(dataModel.GAS_CONTRACT_ACCOUNT);
			return  await electricity ?? await gas;

			async Task<AccountInfo> TryAccount(long? account)
			{
				AccountInfo result = null;
				if (account.HasValue && account.Value > 0)
				{
					var candidate = await _queryResolver.GetAccountInfoByAccountNumber(account.Value.ToString(),true);
					if (GetUniqueId(candidate,moveType) == dataModel.UNIQUE_ID)
					{
						result = candidate; 
					}
				}

				return result;
			}
		}

	}
}