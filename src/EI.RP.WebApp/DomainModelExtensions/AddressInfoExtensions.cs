using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;

namespace EI.RP.WebApp.DomainModelExtensions
{
	
	public static class AddressInfoExtensions
	{
		public static string AsDescriptionText(this AddressInfo src)
		{
			return
				$"{src.CareOf},{string.Join("/", src.HouseNo.Split(' ').Select(x => x.Trim()))} {src.Street.Trim()}, {src.City}, {src.PostalCode}"
					.Trim(',', ' ');
		}
	}
}