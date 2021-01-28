using System;

namespace EI.RP.DataModels.ResidentialPortal
{
	public class MovingHouseProcessStatusDataModel : IEquatable<MovingHouseProcessStatusDataModel>
	{
		#region metadata
		public long ID { get; set; }
		public long UNIQUE_ID { get; set; }

		public DateTime? TIMESTAMP { get; set; }
		#endregion metadata

		#region moveout
		public string MOELECMMREG1 { get; set; }
		public string MOELECMMREG2 { get; set; }
		
		public string MOGASMMREG1 { get; set; }
	
		public long? ELECCONTRACT_ID { get; set; }
		public long? GASCONTRACT_ID { get; set; }
		
		public string MOVEOUT_DATE { get; set; }
		public long? ELEC_CONTRACT_ACCOUNT { get; set; }
		public long? GAS_CONTRACT_ACCOUNT { get; set; }
		public string USERNAME { get; set; }
		public string IS_INCOMINGOCCUPANT { get; set; }
		public string LETTING_AGENTNAME { get; set; }
		public string LETTING_PHONENO { get; set; }
		public bool IS_MOCONFIRM1 { get; set; }
		public bool IS_MOCONFIRM2 { get; set; }
		public string ELEC_PAYMENTMETHOD { get; set; }
		public string GAS_PAYMENTMETHOD { get; set; }
		public string IS_INCOMING_CONFIRM { get; set; }
		#endregion move out
	

		#region move in

		public string MOVEIN_DATE { get; set; }
		public string MIELECMMREG1 { get; set; }
		public string MIELECMMREG2 { get; set; }
		public string MIGASMMREG1 { get; set; }
		public bool IS_MICONFIRM1 { get; set; }
		public bool IS_MICONFIRM2 { get; set; }
		public string MI_PHONENO { get; set; }

		#endregion move in

		#region point of distribution
		public string NEW_MPRN { get; set; }
		public string NEW_GPRN { get; set; }
		#endregion  point of distribution

		#region input prn address
		public string IS_ROI { get; set; }
		public string IS_POBOX { get; set; }
		public string IS_NONROI { get; set; }
		public string ADDRESSLINE1 { get; set; }
		public string ADDRESSLINE2 { get; set; }
		public string ADDRESSLINE3 { get; set; }
		public string TOWN { get; set; }
		public string COUNTY { get; set; }
		public string POST_CODE { get; set; }
		public string COUNTRY { get; set; }
		public string PO_BOX_NUMBER { get; set; }
		public string PO_BOX_POSTCODE { get; set; }
		#endregion address



		public bool Equals(MovingHouseProcessStatusDataModel other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ID == other.ID && UNIQUE_ID == other.UNIQUE_ID && string.Equals(MOELECMMREG1, other.MOELECMMREG1) &&
				   string.Equals(MOELECMMREG2, other.MOELECMMREG2) && string.Equals(MIELECMMREG1, other.MIELECMMREG1) &&
				   string.Equals(MIELECMMREG2, other.MIELECMMREG2) && string.Equals(MOGASMMREG1, other.MOGASMMREG1) &&
				   string.Equals(MIGASMMREG1, other.MIGASMMREG1) && ELECCONTRACT_ID == other.ELECCONTRACT_ID &&
				   GASCONTRACT_ID == other.GASCONTRACT_ID && string.Equals(MOVEIN_DATE, other.MOVEIN_DATE) &&
				   string.Equals(MOVEOUT_DATE, other.MOVEOUT_DATE) &&
				   ELEC_CONTRACT_ACCOUNT == other.ELEC_CONTRACT_ACCOUNT &&
				   GAS_CONTRACT_ACCOUNT == other.GAS_CONTRACT_ACCOUNT && string.Equals(USERNAME, other.USERNAME) &&
				   string.Equals(IS_INCOMINGOCCUPANT, other.IS_INCOMINGOCCUPANT) &&
				   string.Equals(LETTING_AGENTNAME, other.LETTING_AGENTNAME) &&
				   string.Equals(LETTING_PHONENO, other.LETTING_PHONENO) && IS_MOCONFIRM1 == other.IS_MOCONFIRM1 &&
				   IS_MOCONFIRM2 == other.IS_MOCONFIRM2 && IS_MICONFIRM1 == other.IS_MICONFIRM1 &&
				   IS_MICONFIRM2 == other.IS_MICONFIRM2 && string.Equals(MI_PHONENO, other.MI_PHONENO) &&
				   string.Equals(NEW_MPRN, other.NEW_MPRN) && string.Equals(NEW_GPRN, other.NEW_GPRN) &&
				   string.Equals(ELEC_PAYMENTMETHOD, other.ELEC_PAYMENTMETHOD) &&
				   string.Equals(GAS_PAYMENTMETHOD, other.GAS_PAYMENTMETHOD) && IS_ROI == other.IS_ROI &&
				   IS_POBOX == other.IS_POBOX && IS_NONROI == other.IS_NONROI &&
				   string.Equals(ADDRESSLINE1, other.ADDRESSLINE1) && string.Equals(ADDRESSLINE2, other.ADDRESSLINE2) &&
				   string.Equals(ADDRESSLINE3, other.ADDRESSLINE3) && string.Equals(TOWN, other.TOWN) &&
				   string.Equals(COUNTY, other.COUNTY) && string.Equals(POST_CODE, other.POST_CODE) &&
				   string.Equals(COUNTRY, other.COUNTRY) && string.Equals(PO_BOX_NUMBER, other.PO_BOX_NUMBER) &&
				   string.Equals(PO_BOX_POSTCODE, other.PO_BOX_POSTCODE) && TIMESTAMP.Equals(other.TIMESTAMP) &&
				   string.Equals(IS_INCOMING_CONFIRM, other.IS_INCOMING_CONFIRM);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((MovingHouseProcessStatusDataModel) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = ID.GetHashCode();
				hashCode = (hashCode * 397) ^ UNIQUE_ID.GetHashCode();
				hashCode = (hashCode * 397) ^ (MOELECMMREG1 != null ? MOELECMMREG1.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MOELECMMREG2 != null ? MOELECMMREG2.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MIELECMMREG1 != null ? MIELECMMREG1.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MIELECMMREG2 != null ? MIELECMMREG2.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MOGASMMREG1 != null ? MOGASMMREG1.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MIGASMMREG1 != null ? MIGASMMREG1.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ ELECCONTRACT_ID.GetHashCode();
				hashCode = (hashCode * 397) ^ GASCONTRACT_ID.GetHashCode();
				hashCode = (hashCode * 397) ^ (MOVEIN_DATE != null ? MOVEIN_DATE.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MOVEOUT_DATE != null ? MOVEOUT_DATE.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ ELEC_CONTRACT_ACCOUNT.GetHashCode();
				hashCode = (hashCode * 397) ^ GAS_CONTRACT_ACCOUNT.GetHashCode();
				hashCode = (hashCode * 397) ^ (USERNAME != null ? USERNAME.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (IS_INCOMINGOCCUPANT != null ? IS_INCOMINGOCCUPANT.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (LETTING_AGENTNAME != null ? LETTING_AGENTNAME.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (LETTING_PHONENO != null ? LETTING_PHONENO.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ IS_MOCONFIRM1.GetHashCode();
				hashCode = (hashCode * 397) ^ IS_MOCONFIRM2.GetHashCode();
				hashCode = (hashCode * 397) ^ IS_MICONFIRM1.GetHashCode();
				hashCode = (hashCode * 397) ^ IS_MICONFIRM2.GetHashCode();
				hashCode = (hashCode * 397) ^ (MI_PHONENO != null ? MI_PHONENO.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (NEW_MPRN != null ? NEW_MPRN.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (NEW_GPRN != null ? NEW_GPRN.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ELEC_PAYMENTMETHOD != null ? ELEC_PAYMENTMETHOD.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (GAS_PAYMENTMETHOD != null ? GAS_PAYMENTMETHOD.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ IS_ROI.GetHashCode();
				hashCode = (hashCode * 397) ^ IS_POBOX.GetHashCode();
				hashCode = (hashCode * 397) ^ IS_NONROI.GetHashCode();
				hashCode = (hashCode * 397) ^ (ADDRESSLINE1 != null ? ADDRESSLINE1.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ADDRESSLINE2 != null ? ADDRESSLINE2.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ADDRESSLINE3 != null ? ADDRESSLINE3.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TOWN != null ? TOWN.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (COUNTY != null ? COUNTY.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (POST_CODE != null ? POST_CODE.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (COUNTRY != null ? COUNTRY.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PO_BOX_NUMBER != null ? PO_BOX_NUMBER.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PO_BOX_POSTCODE != null ? PO_BOX_POSTCODE.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ TIMESTAMP.GetHashCode();
				hashCode = (hashCode * 397) ^ (IS_INCOMING_CONFIRM != null ? IS_INCOMING_CONFIRM.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(MovingHouseProcessStatusDataModel left, MovingHouseProcessStatusDataModel right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(MovingHouseProcessStatusDataModel left, MovingHouseProcessStatusDataModel right)
		{
			return !Equals(left, right);
		}
	}
}