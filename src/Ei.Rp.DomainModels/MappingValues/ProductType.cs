using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class ProductType : TypedStringValue<ProductType>
	{
	
		[JsonConstructor]
		private ProductType()
		{
		}
		private ProductType(string value) : base(value,disableVerifyValueExists:true)
		{
		}

		public static readonly ProductType RE_PAYG_PENDING_LC = new ProductType("RE_ PAYG_ PENDING_LC");
		public static readonly ProductType RE_PAYG_PENDING_SP = new ProductType("RE_ PAYG_PENDING_SP");
		public static readonly ProductType RE_HOUSEBUDGET = new ProductType("RE_HOUSEBUDGET");
		public static readonly ProductType RE_PAYGC = new ProductType("RE_PAYGC");
		public static readonly ProductType RE_SMARTER_PAYG = new ProductType("RE_SMARTER_PAYG");
		public static readonly ProductType RE_SMARTER_PAYG_HC = new ProductType("RE_SMARTER_PAYG_HC");
		
		public static readonly ProductType RE_HB_ECO = new ProductType("RE_HB_ECO");
		public static readonly ProductType RE_PLUS_HB_ECO = new ProductType("RE_PLUS_HB_ECO");
		public static readonly ProductType RE_HB_SST = new ProductType("RE_HB_SST");
		public static readonly ProductType RE_PLUS_HB_SST = new ProductType("RE_PLUS_HB_SST");
		public static readonly ProductType RE_HOME_ELEC_HB = new ProductType("RE_HOME_ELEC_HB");
		public static readonly ProductType RE_HOME_ELEC_PLUS_HB = new ProductType("RE_HOME_ELEC_PLUS_HB");
		


		public static readonly ProductType RG_PAYG = new ProductType("RG_PAYG");
		public static readonly ProductType RG_PAYGC = new ProductType("RG_PAYGC");

		public static readonly ProductType RE_ELECTRICITYPLAN_WIN = new ProductType("RE_ELECTRICITYPLAN_WIN");
		public static readonly ProductType RE_VR_MOVEIN = new ProductType("RE_VR_MOVEIN");
		public static readonly ProductType RG_GASPLAN_WIN = new ProductType("RG_GASPLAN_WIN");
		public static readonly ProductType RG_GAS_MOVEIN = new ProductType("RG_GAS_MOVEIN");

		public bool IsPAYGPRoduct()
		{
			return this.IsOneOf(
				RE_PAYG_PENDING_LC,
				RE_PAYG_PENDING_SP,
				RE_HB_SST,
				RE_HOME_ELEC_HB,
				RE_HOME_ELEC_PLUS_HB,
				RE_HOUSEBUDGET,
				RE_PAYGC,
				RE_PLUS_HB_SST,
				RE_SMARTER_PAYG,
				RE_SMARTER_PAYG_HC,
				RG_PAYG,
				RG_PAYGC);
		}
		
	}
}