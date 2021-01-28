using System;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using System.Collections.Generic;
using System.Linq;

namespace EI.RP.DomainServices.Commands.Metering.SubmitMeterReading
{
	public class SubmitMeterReadingCommand : DomainCommand, IEquatable<SubmitMeterReadingCommand>
	{
		public SubmitMeterReadingCommand(string accountNumber, 
										 IEnumerable<MeterReadingData> meterReadingDataResults,
										 PointReferenceNumber newPremisePrn = null,
										 bool submitBusinessActivity = true,
										 bool isAddGas = false,
										 bool validateLastResults = false)
		{
			AccountNumber = accountNumber;
			MeterReadingDataResults = meterReadingDataResults;
			NewPremisePrn = newPremisePrn;
			SubmitBusinessActivity = submitBusinessActivity;
			IsAddGas = isAddGas;
			ValidateLastResults = validateLastResults;
		}

		public string AccountNumber { get; }
		public PointReferenceNumber NewPremisePrn { get; }
		public IEnumerable<MeterReadingData> MeterReadingDataResults { get; }
		public bool IsMovingOut() => MeterReadingDataResults.Any(x=>x.Lcpe != null);
		public bool SubmitBusinessActivity { get; }
		public bool IsAddGas { get; }
		public bool ValidateLastResults { get; }

		public bool Equals(SubmitMeterReadingCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(NewPremisePrn, other.NewPremisePrn) && 			
				MeterReadingDataResults.SequenceEqual(other.MeterReadingDataResults) &&
				Equals(SubmitBusinessActivity, other.SubmitBusinessActivity) &&
				IsAddGas == other.IsAddGas &&
				ValidateLastResults == other.ValidateLastResults;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SubmitMeterReadingCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = AccountNumber.GetHashCode();
				hashCode = (hashCode * 397) ^ (MeterReadingDataResults != null ? MeterReadingDataResults.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (NewPremisePrn != null ? NewPremisePrn.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SubmitBusinessActivity.GetHashCode());
				hashCode = (hashCode * 397) ^ (IsAddGas.GetHashCode());
				hashCode = (hashCode * 397) ^ (ValidateLastResults.GetHashCode());
				return hashCode;
			}
		}

		public static bool operator ==(SubmitMeterReadingCommand left, SubmitMeterReadingCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SubmitMeterReadingCommand left, SubmitMeterReadingCommand right)
		{
			return !Equals(left, right);
		}
	}
}