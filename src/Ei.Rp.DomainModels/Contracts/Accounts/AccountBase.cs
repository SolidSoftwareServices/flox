using System;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.Contracts.Accounts
{
	public abstract class AccountBase :IQueryResult, IEquatable<AccountBase>
	{
		protected AccountBase(){}
		protected AccountBase(AccountBase source)
		{
			AccountNumber = source.AccountNumber;
			Partner = source.Partner;
			ClientAccountType = source.ClientAccountType;

			IsOpen = source.IsOpen;
			PointReferenceNumber = source.PointReferenceNumber;
			ContractStatus = source.ContractStatus;
			Description = source.Description;
		}

		public string AccountNumber { get; set; }
		public string Partner { get; set; }
		public ClientAccountType ClientAccountType { get; set; }
		public bool IsOpen { get; set; } = true;
		public PointReferenceNumber PointReferenceNumber { get; set; }
		public ContractStatusType ContractStatus { get; set; }

		public string Description { get; set; }
		public bool IsElectricityAccount()=>ClientAccountType == ClientAccountType.Electricity;
		public bool IsGasAccount() => ClientAccountType == ClientAccountType.Gas;
		public bool IsEnergyServicesAccount() => ClientAccountType == ClientAccountType.EnergyService;

		public bool IsPendingOpening()
		{
			return ContractStatus == ContractStatusType.Pending;
		}

		public override string ToString()
		{
			return $"{nameof(AccountNumber)}: {AccountNumber}, {nameof(Partner)}: {Partner}, {nameof(ClientAccountType)}: {ClientAccountType}, {nameof(IsOpen)}: {IsOpen}, {nameof(PointReferenceNumber)}: {PointReferenceNumber}, {nameof(ContractStatus)}: {ContractStatus}, {nameof(Description)}: {Description}";
		}

		public bool Equals(AccountBase other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return AccountNumber == other.AccountNumber && Partner == other.Partner && Equals(ClientAccountType, other.ClientAccountType) && IsOpen == other.IsOpen && Equals(PointReferenceNumber, other.PointReferenceNumber) && Equals(ContractStatus, other.ContractStatus) && Description == other.Description;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((AccountBase) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Partner != null ? Partner.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ClientAccountType != null ? ClientAccountType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ IsOpen.GetHashCode();
				hashCode = (hashCode * 397) ^ (PointReferenceNumber != null ? PointReferenceNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ContractStatus != null ? ContractStatus.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(AccountBase left, AccountBase right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(AccountBase left, AccountBase right)
		{
			return !Equals(left, right);
		}
	}
}