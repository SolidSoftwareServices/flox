using System;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.Quotations
{
	public class QuotationsInfo : IQueryResult, IEquatable<QuotationsInfo>
	{
		public string QuotationHeaderID { get; set; }
		public string QuotationItemPosition { get; set; }
		public string BusinessAgreementID { get; set; }
		public QuotationStatusType DocumentStatus { get; set; }
		public string ProductID { get; set; }
		public string DivisionID { get; set; }

		public bool Equals(QuotationsInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return QuotationHeaderID == other.QuotationHeaderID && 
				QuotationItemPosition == other.QuotationItemPosition &&
				BusinessAgreementID == other.BusinessAgreementID &&
				DocumentStatus == other.DocumentStatus && 
				ProductID == other.ProductID && 
				DivisionID == other.DivisionID;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((QuotationsInfo) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = QuotationHeaderID.GetHashCode();
				hashCode = (hashCode * 397) ^ (QuotationItemPosition != null ? QuotationItemPosition.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (BusinessAgreementID != null ? BusinessAgreementID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (DocumentStatus != null ? DocumentStatus.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (DivisionID != null ? DivisionID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ProductID != null ? ProductID.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(QuotationsInfo left, QuotationsInfo right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(QuotationsInfo left, QuotationsInfo right)
		{
			return !Equals(left, right);
		}
	}
}