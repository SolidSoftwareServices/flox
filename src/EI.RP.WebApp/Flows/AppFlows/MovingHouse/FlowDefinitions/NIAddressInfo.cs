using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ei.Rp.DomainModels.ReadOnlyCollections;
using Ei.Rp.Mvc.Core.ViewModels.Validations;
using EI.RP.WebApp.Infrastructure.StringResources;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions
{
	public class NIAddressInfo
	{
		public IEnumerable<CountryDetails> CountryList { get; set; }
		public bool IsNIFieldRequired { get; set; }

		[RequiredIf(nameof(IsNIFieldRequired), IfValue = true, ErrorMessage = "You must select a country")]
		public string Country { get; set; }

		public string AddressLine1 { get; set; }

		[RequiredIf(nameof(IsNIFieldRequired), IfValue = true, ErrorMessage = "Please enter house number")]
		[RegularExpression(ReusableRegexPattern.ValidStringWithoutSymbols,
			ErrorMessage = "Please enter a valid house number")]
		public string HouseNumber { get; set; }

		[RequiredIf(nameof(IsNIFieldRequired), IfValue = true, ErrorMessage = "Please enter street")]
		[RegularExpression(ReusableRegexPattern.ValidName, ErrorMessage = "Please enter a valid street")]
		public string Street { get; set; }

		public string AddressLine2 { get; set; }

		[RequiredIf(nameof(IsNIFieldRequired), IfValue = true, ErrorMessage = "You must select a county or state")]
		public string District { get; set; }

		[RequiredIf(nameof(IsNIFieldRequired), IfValue = true, ErrorMessage = "You must provide a town")]
		[RegularExpression(ReusableRegexPattern.ValidName, ErrorMessage = "Please enter a valid town")]
		public string Town { get; set; }

		[RequiredIf(nameof(IsNIFieldRequired), IfValue = true, ErrorMessage = "You must insert a post code")]
		[RegularExpression(ReusableRegexPattern.ValidStringWithoutSymbols,
			ErrorMessage = "Please enter a valid post code")]
		public string PostCode { get; set; }

	}
}