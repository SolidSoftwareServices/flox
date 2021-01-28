using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ei.Rp.DomainModels.ReadOnlyCollections;
using Ei.Rp.Mvc.Core.ViewModels.Validations;
using EI.RP.WebApp.Infrastructure.StringResources;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions
{
	public class POBoxFields
	{
		public IEnumerable<CountryDetails> CountryList { get; set; }
		public bool IsPOBoxFieldRequired { get; set; }
		[RequiredIf(nameof(IsPOBoxFieldRequired), IfValue = true, ErrorMessage = "You must select a country")]
		public string Country { get; set; }
		[RequiredIf(nameof(IsPOBoxFieldRequired), IfValue = true, ErrorMessage = "You must select a county or state")]
		public string District { get; set; }
		[RequiredIf(nameof(IsPOBoxFieldRequired), IfValue = true, ErrorMessage = "You must insert a PO box number")]
		[RegularExpression(ReusableRegexPattern.ValidStringWithoutSymbols, ErrorMessage = "You must enter a valid po box number")]
		public string POBoxNumber { get; set; }
		[RequiredIf(nameof(IsPOBoxFieldRequired), IfValue = true, ErrorMessage = "You must insert a post code")]
		[RegularExpression(ReusableRegexPattern.ValidStringWithoutSymbols, ErrorMessage = "Please enter a valid po box post code")]
		public string POBoxPostCode { get; set; }
	}
}