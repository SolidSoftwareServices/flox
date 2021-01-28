namespace EI.RP.DataModels.ResidentialPortal
{
	public class SmartActivationProductProposal
	{
		public string FreeDayOfElectricityChoice { get; set; }
		public string BundleID { get; set; }
		public string ElecProductID { get; set; }
		public string GasProductID { get; set; }

		public bool Equals(SmartActivationProductProposal other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return FreeDayOfElectricityChoice.Equals(other.FreeDayOfElectricityChoice) &&
				BundleID.Equals(other.BundleID) &&
				ElecProductID.Equals(other.ElecProductID) &&
				GasProductID.Equals(other.GasProductID);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SmartActivationProductProposal)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (FreeDayOfElectricityChoice != null ? FreeDayOfElectricityChoice.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (BundleID != null ? BundleID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ElecProductID != null ? ElecProductID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (GasProductID != null ? GasProductID.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(SmartActivationProductProposal left, SmartActivationProductProposal right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SmartActivationProductProposal left, SmartActivationProductProposal right)
		{
			return !Equals(left, right);
		}
	}
}
