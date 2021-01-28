using System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers
{
    public class GasPointReferenceNumber : PointReferenceNumber
    {
        public const string GPRNRegEx = @"^([0-9]{5,7})$";

        [JsonConstructor]
        public GasPointReferenceNumber(string input)
            : base(input == string.Empty
                    ? string.Empty :
                    input?.PadLeft(7, '0'),
                PointReferenceNumberType.Gprn
                , GPRNRegEx)
        {
        }

        public GasPointReferenceNumber(long input) : this(input.ToString())
        {
            if (input <= 0) throw new ArgumentException();
        }




        public static implicit operator GasPointReferenceNumber(string strValue)
        {
	        if (strValue == null) return null;
			return new GasPointReferenceNumber(strValue);
        }
        public static implicit operator GasPointReferenceNumber(long src)
        {
            return new GasPointReferenceNumber(src);
        }

        public static explicit operator string(GasPointReferenceNumber src)
        {
            return src?.Input;
        }

    }
}