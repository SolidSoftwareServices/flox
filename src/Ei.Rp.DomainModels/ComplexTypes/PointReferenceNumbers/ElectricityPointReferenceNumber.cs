using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers
{
    public class ElectricityPointReferenceNumber : PointReferenceNumber
    {
        public const string OldMPRNRegEx = @"^([0-9]{6})$";
        public const string NewMPRNRegEx = @"^(10[0-9]{9})$";

        [JsonConstructor]
        public ElectricityPointReferenceNumber(string input) : base(input, PointReferenceNumberType.Mprn, OldMPRNRegEx, NewMPRNRegEx)
        {
        }

        public static implicit operator ElectricityPointReferenceNumber(string strValue)
        {
	        if (strValue == null) return null;
            return new ElectricityPointReferenceNumber(strValue);
        }

        public static explicit operator string(ElectricityPointReferenceNumber src)
        {
            return src?.Input;
        }
    }
}