using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.ResidentialPortal;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.InternalShared.MovingHouse;

namespace EI.RP.DomainServices.Queries.MovingHouse.Progress
{
	internal class MoveHouseProgressQueryHandler : QueryHandler<MoveHouseProgressQuery>
	{
		private readonly IResidentialPortalDataRepository _repository;
		private readonly IMovingHouseProgressResolver _resolver;


		public MoveHouseProgressQueryHandler(IResidentialPortalDataRepository repository,
			IMovingHouseProgressResolver resolver)
		{
			_repository = repository;
			_resolver = resolver;
		}

		protected override Type[] ValidQueryResultTypes { get; } =
		{
			typeof(MovingHouseInProgressMovingOutInfo), typeof(MovingHouseInProgressNewPRNsInfo),
			typeof(MovingHouseInProgressMovingInInfo)
		};

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			MoveHouseProgressQuery queryModel)
		{
			IEnumerable<TQueryResult> result;
			var account1 =
				await _repository.GetMovingHouseProcessStatus(_resolver.GetUniqueId(queryModel.InitiatedFromAccount,
					queryModel.MoveType));
			var account2 = queryModel.OtherAccount != null
				? await _repository.GetMovingHouseProcessStatus(_resolver.GetUniqueId(queryModel.OtherAccount,
					queryModel.MoveType))
				: null;

			if (typeof(TQueryResult) == typeof(MovingHouseInProgressMovingOutInfo))
			{
				var info = await MapToMovingOut(account1, account2, queryModel.MoveType);
				result = info.ToOneItemArray().Where(x => x != null).Cast<TQueryResult>();
			}
			else if (typeof(TQueryResult) == typeof(MovingHouseInProgressNewPRNsInfo))
			{
				var info = await MapToNewPrns(account1, account2, queryModel.MoveType);
				result = info.ToOneItemArray().Where(x => x != null).Cast<TQueryResult>();
			}
			else if (typeof(TQueryResult) == typeof(MovingHouseInProgressMovingInInfo))
			{
				var info = await MapToMovingIn(account1, account2, queryModel.MoveType);
				result = info.ToOneItemArray().Where(x => x != null).Cast<TQueryResult>();
			}
			else
			{
				throw new NotImplementedException();
			}

			return result;
		}

		private async Task<MovingHouseInProgressMovingInInfo> MapToMovingIn(
			MovingHouseProcessStatusDataModel account1,
			MovingHouseProcessStatusDataModel account2,
			MovingHouseType moveType)
		{
			var initiatedAccountTask = _resolver.ResolveAccount(account1, moveType);
			var otherAccountTask =
				account2 != null ? _resolver.ResolveAccount(account2, moveType) : Task.FromResult((AccountInfo) null);
			var initiatedAccount = await initiatedAccountTask;
			var otherAccount = await otherAccountTask;
			MovingHouseInProgressMovingInInfo result = null;
			if (initiatedAccount != null || otherAccount != null)
			{
				result = new MovingHouseInProgressMovingInInfo();
				result.ContactNumber = account1.MI_PHONENO;
				result.MovingInDate = account1.MOVEIN_DATE != null
					? DateTime.Parse(account1.MOVEIN_DATE)
					: (DateTime?) null;
				result.HasConfirmedAuthority = account1.IS_MICONFIRM1;
				result.HasConfirmedTermsAndConditions = account1.IS_MICONFIRM2;

				var parsedElectricityMeterReadingDayOr24HrsValue = 0;
				if (!string.IsNullOrEmpty(account1?.MIELECMMREG1))
				{
					parsedElectricityMeterReadingDayOr24HrsValue = int.Parse(account1.MIELECMMREG1);
				}
				else if (!string.IsNullOrEmpty(account2?.MIELECMMREG1)) {
					parsedElectricityMeterReadingDayOr24HrsValue = int.Parse(account2.MIELECMMREG1);
				}
				result.ElectricityMeterReadingDayOr24HrsValue = parsedElectricityMeterReadingDayOr24HrsValue;

				var parsedElectricityMeterReadingNightOrNshValue = 0;
				if (!string.IsNullOrEmpty(account1?.MIELECMMREG2))
				{
					parsedElectricityMeterReadingNightOrNshValue = int.Parse(account1.MIELECMMREG2);
				}
				else if (!string.IsNullOrEmpty(account2?.MIELECMMREG2))
				{
					parsedElectricityMeterReadingNightOrNshValue = int.Parse(account2.MIELECMMREG2);
				}
				result.ElectricityMeterReadingNightOrNshValue = parsedElectricityMeterReadingNightOrNshValue;

				var parsedGasMeterReadingValue = 0;
				if (!string.IsNullOrEmpty(account1?.MIGASMMREG1))
				{
					parsedGasMeterReadingValue = int.Parse(account1.MIGASMMREG1);
				}
				else if (!string.IsNullOrEmpty(account2?.MIGASMMREG1))
				{
					parsedGasMeterReadingValue = int.Parse(account2.MIGASMMREG1);
				}
				result.GasMeterReadingValue = parsedGasMeterReadingValue;
			}

			return result;
		}

		private async Task<MovingHouseInProgressNewPRNsInfo> MapToNewPrns(
			MovingHouseProcessStatusDataModel account1, MovingHouseProcessStatusDataModel account2,
			MovingHouseType moveType)
		{
			var initiatedAccountTask = _resolver.ResolveAccount(account1, moveType);
			var otherAccountTask =
				account2 != null ? _resolver.ResolveAccount(account2, moveType) : Task.FromResult((AccountInfo) null);
			var initiatedAccount = await initiatedAccountTask;
			var otherAccount = await otherAccountTask;
			MovingHouseInProgressNewPRNsInfo result = null;
			if (initiatedAccount != null || otherAccount != null)
			{
				var electricityStatus = initiatedAccount != null && initiatedAccount.IsElectricityAccount()
					? account1
					: otherAccount != null && otherAccount.IsElectricityAccount()
						? account2
						: null;

				var gasStatus = initiatedAccount != null && initiatedAccount.IsGasAccount()
					? account1
					: otherAccount != null && otherAccount.IsGasAccount()
						? account2
						: null;

				result = new MovingHouseInProgressNewPRNsInfo();

				if (electricityStatus != null)
				{
					if (!string.IsNullOrEmpty(electricityStatus.NEW_MPRN)) result.NewMprn = electricityStatus.NEW_MPRN;
					if (!string.IsNullOrEmpty(electricityStatus.NEW_GPRN)) result.NewGprn = electricityStatus.NEW_GPRN;
				}

				if (gasStatus != null)
				{
					if (!string.IsNullOrEmpty(gasStatus.NEW_GPRN)) result.NewGprn = gasStatus.NEW_GPRN;
					if (!string.IsNullOrEmpty(gasStatus.NEW_MPRN)) result.NewMprn = gasStatus.NEW_MPRN;
				}
			}

			return result;
		}

		private async Task<MovingHouseInProgressMovingOutInfo> MapToMovingOut(
			MovingHouseProcessStatusDataModel account1, MovingHouseProcessStatusDataModel account2,
			MovingHouseType moveType)
		{
			var initiatedAccountTask = _resolver.ResolveAccount(account1, moveType);
			var otherAccountTask =
				account2 != null ? _resolver.ResolveAccount(account2, moveType) : Task.FromResult((AccountInfo) null);
			var initiatedAccount = await initiatedAccountTask;
			var otherAccount = await otherAccountTask;
			MovingHouseInProgressMovingOutInfo result = null;
			if (initiatedAccount != null || otherAccount != null)
			{
				var electricityStatus = initiatedAccount != null && initiatedAccount.IsElectricityAccount()
					? account1
					: otherAccount != null && otherAccount.IsElectricityAccount()
						? account2
						: null;

				var gasStatus = initiatedAccount != null && initiatedAccount.IsGasAccount()
					? account1
					: otherAccount != null && otherAccount.IsGasAccount()
						? account2
						: null;

				result = new MovingHouseInProgressMovingOutInfo();

				result.InitiatedFromAccountNumber = initiatedAccount?.AccountNumber;
				result.OtherAccountNumber = otherAccount?.AccountNumber;

				if (electricityStatus != null)
				{
					result.ElectricityMeterReadingDayOr24HrsValue =
						int.Parse(electricityStatus.MOELECMMREG1 ?? 0.ToString());
					result.ElectricityMeterReadingNightOrNshValue =
						int.Parse(electricityStatus.MOELECMMREG2 ?? 0.ToString());
				}

				if (gasStatus != null) result.GasMeterReadingValue = int.Parse(gasStatus.MOGASMMREG1 ?? 0.ToString());

				if (account1 != null)
				{
					result.IncomingOccupant = account1.IS_INCOMINGOCCUPANT.ToBooleanFromYesNoString();
					result.TermsAndConditionsAccepted = account1.IS_MOCONFIRM1;
					result.InformationCollectionAuthorized = account1.IS_MOCONFIRM2;

					result.LettingAgentName = account1.LETTING_AGENTNAME;
					result.LettingPhoneNumber = account1.LETTING_PHONENO;
					result.MovingOutDate = DateTime.Parse(account1.MOVEOUT_DATE);
					result.OccupierDetailsAccepted = bool.Parse(account1.IS_INCOMING_CONFIRM);
				}
			}

			return result;
		}
	}
}