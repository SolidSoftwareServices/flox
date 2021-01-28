using System;
using System.Linq;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;

namespace EI.RP.DataModels.Sap.CrmUmc.Dtos.Extensions
{
	public static class AddressInfoDtoExtensions
	{
		public static bool IsEquivalentTo(this AddressInfoDto src, AddressInfoDto other)
		{
			if (src == null) throw new ArgumentNullException(nameof(src));
			if (other == null) throw new ArgumentNullException(nameof(other));

			var houseNo = src.HouseSupplement == string.Empty ? src.HouseNo : src.HouseSupplement;
			var house1 = GetHouseNo(houseNo);
			var remainingAddress1 = house1 == string.Empty ? src.ShortForm : src.ShortForm.Replace(house1, string.Empty).Trim();

			var house = other.HouseSupplement == string.Empty ? other.HouseNo : other.HouseSupplement;
			var house2 = GetHouseNo(house);
			
			var remainingAddress2 = house2 == string.Empty ? other.ShortForm : other.ShortForm.Replace(house2, string.Empty).Trim();
			return other.ShortForm.Equals( src.ShortForm,StringComparison.InvariantCultureIgnoreCase) || remainingAddress1 == remainingAddress2;

			string GetHouseNo(string value)
			{
				value = value.ToLower();
				value = value.Replace("apt", string.Empty);
				value = value.Replace("flat", string.Empty);
				value = value.Replace("no", string.Empty);
				value = value.Replace(".", string.Empty);
				value = value.Replace("flats", string.Empty);
				value = value.Trim();
				return value;
			}
		}

		public static string AsDescriptionText(this AddressInfoDto src)
		{

			return $"{src.CareOf},{string.Join("/", src.HouseNo.Split(' ').Select(x => x.Trim()))} {src.Street.Trim()},{src.City},{src.PostalCode}".Trim(',', ' ');


		}
	}
}