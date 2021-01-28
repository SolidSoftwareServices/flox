using System;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using FluentValidation.Validators;

namespace Ei.Rp.DomainModels.Metering.Consumption
{
	public partial class AccountConsumption
	{
		public abstract class AccountConsumptionEntry<TValue>
		{
			public DateTime Date { get; set; }

			public string Prn { get; set; }

			public string SerialNumber { get; set; }

			public TValue Value { get; set; }

		

			public abstract EntryType Type { get; }

			public enum EntryType
			{
				CostInEuro = 1,
				UsageInKwh = 2
			}

			public override string ToString()
			{
				return $"{nameof(Date)}: {Date}, {nameof(Prn)}: {Prn}, {nameof(SerialNumber)}: {SerialNumber}, {nameof(Value)}: {Value}, {nameof(Type)}: {Type}";
			}
		}

		public class CostEntry : AccountConsumptionEntry<EuroMoney>
		{
			public override EntryType Type { get; } = EntryType.CostInEuro;

			public static CostEntry From(CostEntry src)
			{
				if (src == null) return null;
				return new CostEntry
				{
					Date = src.Date,
					Prn=src.Prn,
					SerialNumber=src.SerialNumber,
					Value=src.Value
				};
			}
		}

		public class UsageEntry : AccountConsumptionEntry<decimal>
		{
			public override EntryType Type { get; } = EntryType.UsageInKwh;

			public static UsageEntry From(UsageEntry src)
			{

				if (src == null) return null;
				return new UsageEntry
				{
					Date = src.Date,
					Prn = src.Prn,
					SerialNumber = src.SerialNumber,
					Value = src.Value
				};
			}
		}

	}
}