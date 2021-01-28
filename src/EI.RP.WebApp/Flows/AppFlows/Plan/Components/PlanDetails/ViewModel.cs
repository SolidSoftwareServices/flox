using System;
using System.ComponentModel;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.Plan.Components.PlanDetails
{
    public class ViewModel : FlowComponentViewModel
    {
	    public string AccountNumber
	    {
			get; 
			set;
	    }

		[DisplayName("Discount")]
        public decimal Discount
        {
            get;
            set;
        }

		[DisplayName("Current price plan")]
        public string PlanName
        {
	        get;
	        set;
        }

        public bool CanAddGas
        {
	        get; 
	        set;
        }

		public UpgradeInfo Upgrading { get; }=new UpgradeInfo();

        public class UpgradeInfo : IEquatable<UpgradeInfo>
        {
	        public bool IsUpgradeToSmartAvailable { get; set; }

	        public string UpgradePlanName { get; set; }

	        public bool Equals(UpgradeInfo other)
	        {
		        if (ReferenceEquals(null, other)) return false;
		        if (ReferenceEquals(this, other)) return true;
		        return IsUpgradeToSmartAvailable == other.IsUpgradeToSmartAvailable && UpgradePlanName == other.UpgradePlanName;
	        }

	        public override bool Equals(object obj)
	        {
		        if (ReferenceEquals(null, obj)) return false;
		        if (ReferenceEquals(this, obj)) return true;
		        if (obj.GetType() != this.GetType()) return false;
		        return Equals((UpgradeInfo) obj);
	        }

	        public override int GetHashCode()
	        {
		        return HashCode.Combine(IsUpgradeToSmartAvailable, UpgradePlanName);
	        }

	        public static bool operator ==(UpgradeInfo left, UpgradeInfo right)
	        {
		        return Equals(left, right);
	        }

	        public static bool operator !=(UpgradeInfo left, UpgradeInfo right)
	        {
		        return !Equals(left, right);
	        }

	        public override string ToString()
	        {
		        return $"{nameof(IsUpgradeToSmartAvailable)}: {IsUpgradeToSmartAvailable}, {nameof(UpgradePlanName)}: {UpgradePlanName}";
	        }
        }
    }
}