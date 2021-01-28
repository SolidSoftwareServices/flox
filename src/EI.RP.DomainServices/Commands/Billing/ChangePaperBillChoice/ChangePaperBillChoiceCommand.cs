using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainServices.Commands.Billing.ChangePaperBillChoice
{
	public sealed class ChangePaperBillChoiceCommand : DomainCommand, IEquatable<ChangePaperBillChoiceCommand>
	{
		
		public ChangePaperBillChoiceCommand(string accountNumber,PaperBillChoice newChoice)
		{
			AccountNumber = accountNumber;
			NewChoice = newChoice;
		}

		public string AccountNumber { get; }
		public PaperBillChoice NewChoice { get; }

		public bool Equals(ChangePaperBillChoiceCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return AccountNumber == other.AccountNumber && Equals(NewChoice, other.NewChoice);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ChangePaperBillChoiceCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((AccountNumber != null ? AccountNumber.GetHashCode() : 0) * 397) ^ (NewChoice != null ? NewChoice.GetHashCode() : 0);
			}
		}

		public static bool operator ==(ChangePaperBillChoiceCommand left, ChangePaperBillChoiceCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ChangePaperBillChoiceCommand left, ChangePaperBillChoiceCommand right)
		{
			return !Equals(left, right);
		}
	}
}