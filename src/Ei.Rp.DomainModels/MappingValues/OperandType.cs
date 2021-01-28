using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class OperandType : TypedStringValue<OperandType>
    {
        [JsonConstructor]
        private OperandType()
        {
        }
        private OperandType(string value) : base(value)
        {
        }

        public static readonly OperandType FirstStaffDiscount = new OperandType("EI_FC_ST1");
		public static readonly OperandType FreeElectricityAllowance = new OperandType("FEA_DSFA");
        public static readonly OperandType AnnualConsumption  = new OperandType("ANNUALCONS");
        public static readonly OperandType DuosGroup = new OperandType("DUOS_GROUP");
        public static readonly OperandType SmartMeterConfiguration = new OperandType("MTR_CNG_CD");
        public static readonly OperandType CTFValue = new OperandType("CTFVALUE");
        public static readonly OperandType Band = new OperandType("BAND");
	}
}