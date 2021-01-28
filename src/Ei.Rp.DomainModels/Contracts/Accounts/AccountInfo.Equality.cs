using System;

namespace Ei.Rp.DomainModels.Contracts.Accounts
{
    public partial class AccountInfo : IEquatable<AccountInfo>
    {
	    public bool Equals(AccountInfo other)
	    {
		    if (ReferenceEquals(null, other)) return false;
		    if (ReferenceEquals(this, other)) return true;
		    return base.Equals(other) && IsPAYGCustomer == other.IsPAYGCustomer && Name == other.Name &&
		           FirstName == other.FirstName && LastName == other.LastName && 
		           ContractId == other.ContractId && PremiseConnectionObjectId == other.PremiseConnectionObjectId &&
		           Equals(PaymentMethod, other.PaymentMethod) &&
		           Nullable.Equals(ContractStartDate, other.ContractStartDate) &&
		           Nullable.Equals(ContractEndDate, other.ContractEndDate) &&
		           Equals(PaperBillChoice, other.PaperBillChoice) && BundleReference == other.BundleReference &&
		           Equals(BankAccounts, other.BankAccounts) && BillToAccountAddressId == other.BillToAccountAddressId &&
		           IsLossInProgress == other.IsLossInProgress &&
		           SwitchToSmartPlanDismissed == other.SwitchToSmartPlanDismissed &&
		           Equals(SmartPeriods, other.SmartPeriods) && Equals(NonSmartPeriods, other.NonSmartPeriods) &&
		           PlanName == other.PlanName && DiscountAppliedPercentage == other.DiscountAppliedPercentage &&
		           Equals(BusinessAgreement, other.BusinessAgreement) &&
		           Equals(IncomingBankAccount, other.IncomingBankAccount) &&
		           SmartActivationStatus == other.SmartActivationStatus &&
		           HasStaffDiscountApplied == other.HasStaffDiscountApplied &&
		           HasQuotationsInProgress == other.HasQuotationsInProgress &&
		           CanSwitchToStandardPlan == other.CanSwitchToStandardPlan &&
		           CanSubmitMeterReading == other.CanSubmitMeterReading;
	    }

	    public override bool Equals(object obj)
	    {
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != this.GetType()) return false;
		    return Equals((AccountInfo) obj);
	    }

	    public override int GetHashCode()
	    {
		    unchecked
		    {
			    int hashCode = base.GetHashCode();
			    hashCode = (hashCode * 397) ^ IsPAYGCustomer.GetHashCode();
			    hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (LastName != null ? LastName.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (ContractId != null ? ContractId.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (PremiseConnectionObjectId != null ? PremiseConnectionObjectId.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (PaymentMethod != null ? PaymentMethod.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ ContractStartDate.GetHashCode();
			    hashCode = (hashCode * 397) ^ ContractEndDate.GetHashCode();
			    hashCode = (hashCode * 397) ^ (PaperBillChoice != null ? PaperBillChoice.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (BundleReference != null ? BundleReference.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (BankAccounts != null ? BankAccounts.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (BillToAccountAddressId != null ? BillToAccountAddressId.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ IsLossInProgress.GetHashCode();
			    hashCode = (hashCode * 397) ^ SwitchToSmartPlanDismissed.GetHashCode();
			    hashCode = (hashCode * 397) ^ (SmartPeriods != null ? SmartPeriods.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (NonSmartPeriods != null ? NonSmartPeriods.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (PlanName != null ? PlanName.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ DiscountAppliedPercentage.GetHashCode();
			    hashCode = (hashCode * 397) ^ (BusinessAgreement != null ? BusinessAgreement.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (IncomingBankAccount != null ? IncomingBankAccount.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (int) SmartActivationStatus;
			    hashCode = (hashCode * 397) ^ HasStaffDiscountApplied.GetHashCode();
			    hashCode = (hashCode * 397) ^ HasQuotationsInProgress.GetHashCode();
			    hashCode = (hashCode * 397) ^ CanSwitchToStandardPlan.GetHashCode();
			    hashCode = (hashCode * 397) ^ CanSubmitMeterReading.GetHashCode();
			    return hashCode;
		    }
	    }

	    public static bool operator ==(AccountInfo left, AccountInfo right)
	    {
		    return Equals(left, right);
	    }

	    public static bool operator !=(AccountInfo left, AccountInfo right)
	    {
		    return !Equals(left, right);
	    }
    }
}