using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class ContractStatusType : TypedStringValue<ContractStatusType>
	{
		[JsonConstructor]
		private ContractStatusType()
		{
		}
		private ContractStatusType(string value, string debuggerFriendlyDisplayValue) : base(value, debuggerFriendlyDisplayValue,true)
		{
		}

		public static readonly ContractStatusType Unknown = new ContractStatusType(string.Empty, nameof(Unknown));
		public static readonly ContractStatusType Accepted = new ContractStatusType("ZACC", nameof(Accepted));
		public static readonly ContractStatusType Active = new ContractStatusType("ZACT", nameof(Active));
		public static readonly ContractStatusType CancellationinProgress = new ContractStatusType("ZCIP", nameof(CancellationinProgress));
		public static readonly ContractStatusType CancellationRejAcceptance = new ContractStatusType("ZCRA", nameof(CancellationRejAcceptance));
		public static readonly ContractStatusType CancellationRejected = new ContractStatusType("ZCRJ", nameof(CancellationRejected));
		public static readonly ContractStatusType CancellationResubmitted = new ContractStatusType("ZCRS", nameof(CancellationResubmitted));
		public static readonly ContractStatusType Cancelled = new ContractStatusType("ZCAN", nameof(Cancelled));
		public static readonly ContractStatusType Confirmed = new ContractStatusType("ZCON", nameof(Confirmed));
		public static readonly ContractStatusType Inactive = new ContractStatusType("ZINA", nameof(Inactive));
		public static readonly ContractStatusType Incomplete = new ContractStatusType("ZINC", nameof(Incomplete));
		public static readonly ContractStatusType LossConfirmed = new ContractStatusType("ZLCN", nameof(LossConfirmed));
		public static readonly ContractStatusType LossinProgress = new ContractStatusType("ZLIP", nameof(LossinProgress));
		public static readonly ContractStatusType LossObjectioninProgress = new ContractStatusType("ZLOB", nameof(Inactive));
		public static readonly ContractStatusType LossObjectionRejected = new ContractStatusType("ZLOR", nameof(LossObjectionRejected));
		public static readonly ContractStatusType LossObjectionWithdrawn = new ContractStatusType("ZLOW", nameof(LossObjectionWithdrawn));
		public static readonly ContractStatusType Pending = new ContractStatusType("ZPEN", nameof(Pending));
		public static readonly ContractStatusType ProvisionallyAccepted = new ContractStatusType("ZPRO", nameof(ProvisionallyAccepted));
		public static readonly ContractStatusType Rejected = new ContractStatusType("ZREJ", nameof(Rejected));
		public static readonly ContractStatusType Resubmitted = new ContractStatusType("ZRES", nameof(Resubmitted));
		public static readonly ContractStatusType Suspended = new ContractStatusType("ZSUS", nameof(Suspended));

		public bool IsAcquisitionCompletedState()
		{
			return IsOneOf(Active, Inactive, LossConfirmed, LossinProgress, LossObjectioninProgress, LossObjectionRejected, LossObjectionWithdrawn);
		}

	}
}
