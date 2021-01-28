using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainServices.Queries.Contracts.Accounts
{
	public class AccountInfoQuery : IQueryModel, IEquatable<AccountInfoQuery>
	{
		public QueryCacheResultsType CacheResults => QueryCacheResultsType.UserSpecific;
		

		public string AccountNumber { get; set; }
		public PointReferenceNumber Prn { get; set; }

		public ClientAccountType AccountType { get; set; }

		public bool? Opened { get; set; } = null;

		public bool RetrieveDuelFuelSisterAccounts { get; set; } = false;

		public string BusinessPartner { get; set; }

		/// <summary>
		/// DualFuel accounts can have more then one contract. 
		/// In theory they should have same/very similar addresses but we found SAP cases where addresses are different.
		/// In BAU system we found they ignore address mismatch and we have to introduce this flag for switch off/on address matching validation.
		/// </summary>
		public bool IgnoreDuelFuelAddressMismatch { get; set; } = true;

		public bool RetrievesAll()
		{
			return AccountNumber == null && Prn == null && Opened == null && !RetrieveDuelFuelSisterAccounts &&
			       BusinessPartner == null && AccountType==null;
		}
		public bool IsValidQuery(out string[] notValidReasons)
		{
			var result=new List<string>();

			if (RetrieveDuelFuelSisterAccounts 
			    && string.IsNullOrWhiteSpace(AccountNumber) 
			    && Prn==null)
			{
				result.Add("To retrieve the  duel fuel specify account number or m(g)prn");
			}

			if (!string.IsNullOrWhiteSpace(AccountNumber)
			    && Prn!=null )
			{
				result.Add("Cannot specify m(g)prn and the account number in the same query.");
			}
			if (!string.IsNullOrWhiteSpace(AccountNumber)
			    && AccountType!=null )
			{
				result.Add("Cannot specify account Type and the account number in the same query.");
			}

			if (Prn!=null && AccountType!=null )
			{
				result.Add("Cannot specify account Type and the prn in the same query.");
			}
			if (!string.IsNullOrWhiteSpace(AccountNumber)
			    && BusinessPartner != null)
			{
				result.Add("Cannot specify business partner and the account number in the same query.");
			}
			notValidReasons = result.ToArray();
			return !notValidReasons.Any();
		}

		public bool Equals(AccountInfoQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return AccountNumber == other.AccountNumber 
			       && Equals(Prn, other.Prn) 
			       && Equals(AccountType, other.AccountType)
			       && Opened == other.Opened
			       && RetrieveDuelFuelSisterAccounts == other.RetrieveDuelFuelSisterAccounts
			       && BusinessPartner == other.BusinessPartner
			       && IgnoreDuelFuelAddressMismatch == other.IgnoreDuelFuelAddressMismatch;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((AccountInfoQuery) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Prn != null ? Prn.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (AccountType != null ? AccountType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ Opened.GetHashCode();
				hashCode = (hashCode * 397) ^ RetrieveDuelFuelSisterAccounts.GetHashCode();
				hashCode = (hashCode * 397) ^ (BusinessPartner != null ? BusinessPartner.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ IgnoreDuelFuelAddressMismatch.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(AccountInfoQuery left, AccountInfoQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(AccountInfoQuery left, AccountInfoQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(CacheResults)}: {CacheResults}, {nameof(AccountNumber)}: {AccountNumber}, {nameof(Prn)}: {Prn}, {nameof(AccountType)}: {AccountType}, {nameof(Opened)}: {Opened}, {nameof(RetrieveDuelFuelSisterAccounts)}: {RetrieveDuelFuelSisterAccounts}, {nameof(BusinessPartner)}: {BusinessPartner}, {nameof(IgnoreDuelFuelAddressMismatch)}: {IgnoreDuelFuelAddressMismatch}";
		}


		
	}
}