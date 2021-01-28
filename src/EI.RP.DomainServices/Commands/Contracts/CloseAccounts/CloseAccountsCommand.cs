using System;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Commands.Contracts.CloseAccounts.Model;
using EI.RP.DomainServices.Commands.Contracts.CloseAccounts.Services;

namespace EI.RP.DomainServices.Commands.Contracts.CloseAccounts
{
	public sealed partial  class CloseAccountsCommand : DomainCommand
	{
		public CloseAccountsCommand(ClientAccountType contractAccountType, AddressInfo addressInfo,
			DateTime moveOutDate, ElectricityMeterReading meterReadingElectricityAccount = null,
			GasMeterReading meterReadingGasAccount = null,
			MoveOutIncommingOccupantInfo moveOutIncommingOccupantInfo = null)
		{
			if (meterReadingElectricityAccount == null && meterReadingGasAccount == null) throw new ArgumentException("Account not specified");
			if (contractAccountType == ClientAccountType.Electricity && meterReadingElectricityAccount == null ||
			    contractAccountType == ClientAccountType.Gas && meterReadingGasAccount == null)
				throw new ArgumentException("Invalid contract type");


			ContractAccountType = contractAccountType;
			MeterReadingElectricityAccount = meterReadingElectricityAccount;
			AddressInfo = addressInfo;
			MoveOutDate = moveOutDate;
			MeterReadingGasAccount = meterReadingGasAccount;
			MoveOutIncommingOccupant = moveOutIncommingOccupantInfo;
		}

		public ClientAccountType ContractAccountType { get; }
		public ElectricityMeterReading MeterReadingElectricityAccount { get; }
		public GasMeterReading MeterReadingGasAccount { get; }
		public AddressInfo AddressInfo { get; }

		public MoveOutIncommingOccupantInfo MoveOutIncommingOccupant { get; }
		public DateTime MoveOutDate { get; }

		internal CloseAccountInfo Context { get; set; }
	}
}