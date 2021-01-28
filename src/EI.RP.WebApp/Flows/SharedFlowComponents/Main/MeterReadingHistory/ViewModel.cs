using Ei.Rp.DomainModels.MappingValues;

using EI.RP.WebApp.Flows.AppFlows;
using System;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.MeterReadingHistory
{
	public class ViewModel : FlowPageableComponentViewModel<ViewModel.MeterRead>
	{
		public ClientAccountType AccountType { get; set; }

		public ResidentialPortalFlowType ContainedInFlowType { get; set; }

		public string TableId { get; set; }
		public string PaginationId { get; set; }
		public string WhenChangingPageFocusOn { get; set; }

		public class MeterRead
		{
			public string MeterNumber { get; set; }
			public string MeterType { get; set; }
			public string Multiplier { get; set; }
			public DateTime? FromDate { get; set; }
			public DateTime? ToDate { get; set; }
			public string Reading { get; set; }
			public string MeterReadingType { get; set; }
			public string Consumption { get; set; }
			public string ConversionFactor { get; set; }
			public string Unit { get; set; }
			public string MeasurementUnitForConsumption { get; set; }
			public bool IsEstimate { get; set; }
			public DateTime Date { get; set; }
			public DateTime MoveInDate { get; set; }
			public bool IsPendingReadingVerification { get; set; }
			public string MaskedSerialNumber { get; set; }
			public string SerialNumber { get; set; }
		}
	}
}