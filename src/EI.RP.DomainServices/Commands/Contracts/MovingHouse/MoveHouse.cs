using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.Context;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse
{
	public sealed class MoveHouse : DomainCommand, IEquatable<MoveHouse>
	{
	
		public MoveHouse(
			string electricityAccount,
			string gasAccount, 
			MovingHouseType moveType,
			IEnumerable<SetUpDirectDebitDomainCommand> commandsToExecute, 
			ClientAccountType contractAccountType)
		{
			if (electricityAccount == null && gasAccount == null)
				throw new ArgumentException("Must specify one account");
			
			ElectricityAccount = electricityAccount;
			GasAccount = gasAccount;
			MoveType = moveType ?? throw new ArgumentNullException(nameof(moveType));
			CommandsToExecute = commandsToExecute??new SetUpDirectDebitDomainCommand[0];
			ContractAccountType = contractAccountType;
		}

		public string ElectricityAccount { get; }

		public string GasAccount { get; }
		public MovingHouseType MoveType { get; }

		//if the command exists is setup new dd otherwise is manual payment
		public IEnumerable<SetUpDirectDebitDomainCommand> CommandsToExecute { get; }
		public ClientAccountType ContractAccountType { get; }
		internal CompleteMoveHouseContext Context { get; set; }

		internal bool IsAddingElectricity()
		{
			return MoveType.IsOneOf(MovingHouseType.MoveGasAndAddElectricity);
		}

		public bool IsAddingGas()
		{
			return MoveType.IsOneOf(MovingHouseType.MoveElectricityAndAddGas);
		}

		public bool Equals(MoveHouse other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return Equals(ElectricityAccount, other.ElectricityAccount) &&
				Equals(GasAccount, other.GasAccount) &&
				Equals(MoveType, other.MoveType) &&
				Equals(ContractAccountType, other.ContractAccountType) &&
				CommandsToExecute.SequenceEqual(other.CommandsToExecute);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((MoveHouse) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (ElectricityAccount != null ? ElectricityAccount.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (GasAccount != null ? GasAccount.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MoveType != null ? MoveType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (CommandsToExecute != null ? CommandsToExecute.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ContractAccountType != null ? ContractAccountType.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(MoveHouse left, MoveHouse right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(MoveHouse left, MoveHouse right)
		{
			return !Equals(left, right);
		}
	}
}