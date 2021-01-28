using System.Collections.Generic;
using System.Linq;

namespace EI.RP.DataModels.Sap.CrmUmc.Dtos
{
	public partial class ProductProposalResultDto
	{
		private sealed class IdsEqualityComparer : IEqualityComparer<ProductProposalResultDto>
		{
			public bool Equals(ProductProposalResultDto x, ProductProposalResultDto y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return x.BundleID == y.BundleID && x.ElecProductID == y.ElecProductID && x.GasProductID == y.GasProductID;
			}

			public int GetHashCode(ProductProposalResultDto obj)
			{
				unchecked
				{
					var hashCode = (obj.BundleID != null ? obj.BundleID.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (obj.ElecProductID != null ? obj.ElecProductID.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (obj.GasProductID != null ? obj.GasProductID.GetHashCode() : 0);
					return hashCode;
				}
			}
		}

		private static IEqualityComparer<ProductProposalResultDto> IdsComparer { get; } = new IdsEqualityComparer();


		private static readonly IEnumerable<ProductProposalResultDto> NonSmartProducts = new[]
		{
			new ProductProposalResultDto{BundleID = "RDF_DUALFUELPLAN_WIN",ElecProductID = "RE_ELECTRICITYPLAN_WIN",GasProductID = "RG_GASPLAN_WIN"},
			new ProductProposalResultDto{BundleID = "RDF_DUALFUELPLAN",ElecProductID = "RE_ELECTRICITYPLAN",GasProductID = "RG_GASPLAN"}
		};
		public bool IsNonSmartProduct()
		{
			return NonSmartProducts.Any(x => IdsComparer.Equals(this, x));
		}

		private static readonly IEnumerable<ProductProposalResultDto> StandardProducts = new[]
		{
			new ProductProposalResultDto{ElecProductID = "RE_HOME_ELEC"},
			new ProductProposalResultDto{BundleID = "RDF_HOME_ELEC",ElecProductID = "RE_HOME_ELEC",GasProductID = "RG_GASPLAN"},
		};

		public bool IsStandardProduct()
		{
			return StandardProducts.Any(x => IdsComparer.Equals(this, x));
		}
	}
}