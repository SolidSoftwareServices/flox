using System;
using System.Globalization;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.ResidentialPortal;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.InternalShared.MovingHouse;

namespace EI.RP.DomainServices.Commands.LongRunningFlows.MovingHouse
{
	internal class RecordProgressCommandHandler : ICommandHandler<RecordMovingOutProgress>, ICommandHandler<RecordMovingHomePrns>, ICommandHandler<RecordMovingInProgress>
	{
		private readonly IResidentialPortalDataRepository _portalDataRepository;
		private readonly IMovingHouseProgressResolver _resolver;
		private readonly IUserSessionProvider _sessionProvider;

		public RecordProgressCommandHandler(IResidentialPortalDataRepository portalDataRepository, IMovingHouseProgressResolver resolver,IUserSessionProvider sessionProvider)
		{
			_portalDataRepository = portalDataRepository;
			_resolver = resolver;
			_sessionProvider = sessionProvider;
		}

		public async Task ExecuteAsync(RecordMovingOutProgress command)
		{
			await Task.WhenAll(_ExecuteFor(command.ElectricityAccount(),command,MapMovingOut)
				,_ExecuteFor(command.GasAccount(), command, MapMovingOut));
		}

		public async Task ExecuteAsync(RecordMovingHomePrns command)
		{
			await Task.WhenAll(_ExecuteFor(command.ElectricityAccount(), command, MapPrns)
				, _ExecuteFor(command.GasAccount(), command, MapPrns));
		}

		public async Task ExecuteAsync(RecordMovingInProgress command)
		{
			await Task.WhenAll(_ExecuteFor(command.ElectricityAccount(), command, MapMovingIn)
				, _ExecuteFor(command.GasAccount(), command, MapMovingIn));
		}

		private async Task _ExecuteFor<TCommand>(AccountInfo account, TCommand commandData,Action<AccountInfo,TCommand, MovingHouseProcessStatusDataModel> mapper)
		where TCommand: RecordMovingHomeProgress
		{
			if (account == null) return;
			var uniqueId = _resolver.GetUniqueId(account,commandData.MoveType);
			var dataModel = await _portalDataRepository.GetMovingHouseProcessStatus(uniqueId) ?? new MovingHouseProcessStatusDataModel();
			
			mapper(account,commandData,dataModel);
			dataModel = await _portalDataRepository.SetMovingHouseProcessStatus(dataModel);
			return;


		
		}
		void MapMovingOut(AccountInfo account,RecordMovingOutProgress commandData, MovingHouseProcessStatusDataModel dataModel)
		{

			var uniqueId = _resolver.GetUniqueId(account,commandData.MoveType);
			var isNew = dataModel.ID == 0;
			dataModel.UNIQUE_ID = uniqueId;

			MapElectricity();
			MapGas();
			dataModel.IS_INCOMINGOCCUPANT = commandData.IncomingOccupant.ToYesNoString();
			dataModel.LETTING_AGENTNAME = commandData.LettingAgentName;
			dataModel.LETTING_PHONENO = commandData.LettingPhoneNumber;
			dataModel.MOVEOUT_DATE = commandData.MovingOutDate.ToString("O");
			dataModel.IS_MOCONFIRM1 = commandData.TermsAndConditionsAccepted;
			dataModel.IS_MOCONFIRM2 = commandData.InformationCollectionAuthorized;
			dataModel.IS_INCOMING_CONFIRM = commandData.OccupierDetailsAccepted.ToString();

			if (isNew)
			{
				dataModel.USERNAME = _sessionProvider.UserName;
			}

			void MapElectricity()
			{
				if (account.IsElectricityAccount())
				{
					dataModel.MOELECMMREG1 = commandData.ElectricityMeterReading24HrsOrDayValue.ToString(CultureInfo.InvariantCulture);
					dataModel.MOELECMMREG2 = commandData.ElectricityMeterReadingNightOrNshValue.ToString(CultureInfo.InvariantCulture);
					dataModel.ELECCONTRACT_ID = (account.ContractId).ToLong();
					dataModel.ELEC_CONTRACT_ACCOUNT = (account.AccountNumber).ToLong();
					if (isNew)
					{
						dataModel.ELEC_PAYMENTMETHOD = account.PaymentMethod ?? string.Empty;
					}
				}
			}

			void MapGas()
			{
				if (account.IsGasAccount())
				{
					dataModel.MOGASMMREG1 = commandData.GasMeterReadingValue.ToString(CultureInfo.InvariantCulture);
					dataModel.GASCONTRACT_ID = (account.ContractId).ToLong();
					dataModel.GAS_CONTRACT_ACCOUNT = (account.AccountNumber).ToLong();
					if (isNew)
					{
						dataModel.GAS_PAYMENTMETHOD = account.PaymentMethod ?? string.Empty;
					}
				}
			}
		}
		void MapPrns(AccountInfo account, RecordMovingHomePrns commandData, MovingHouseProcessStatusDataModel dataModel)
		{

			var uniqueId = _resolver.GetUniqueId(account, commandData.MoveType);
			dataModel.UNIQUE_ID = uniqueId;
			
			if (!string.IsNullOrEmpty(commandData.NewMprn?.ToString()))
			{
				dataModel.NEW_MPRN = commandData.NewMprn?.ToString();
			}

			if (!string.IsNullOrEmpty(commandData.NewGprn?.ToString()))
			{
				dataModel.NEW_GPRN = commandData.NewGprn?.ToString();
			}			
		}

		void MapMovingIn(AccountInfo account, RecordMovingInProgress commandData, MovingHouseProcessStatusDataModel dataModel)
		{

			var uniqueId = _resolver.GetUniqueId(account, commandData.MoveType);
			dataModel.UNIQUE_ID = uniqueId;
			dataModel.MI_PHONENO = commandData.ContactNumber;
			dataModel.MOVEIN_DATE = commandData.MovingInDate.ToString("O");
			dataModel.IS_MICONFIRM1 = commandData.HasConfirmedAuthority;
			dataModel.IS_MICONFIRM2 = commandData.HasConfirmedTermsAndConditions;

			MapElectricity();
			MapGas();
			
			void MapElectricity()
			{
				dataModel.MIELECMMREG1 =
					commandData.ElectricityMeterReading24HrsOrDayValue.ToString(CultureInfo.InvariantCulture);
				dataModel.MIELECMMREG2 = commandData.ElectricityMeterReadingNightValueOrNshValue.ToString(CultureInfo.InvariantCulture);
			}

			void MapGas()
			{
				dataModel.MIGASMMREG1 = commandData.GasMeterReadingValue.ToString(CultureInfo.InvariantCulture);
			}
		}

	}
}