using System;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.Billing
{

	public class NextBillEstimation : IQueryResult
	{
		[JsonProperty]
		private EuroMoney _estimatedAmount;
		public string AccountNumber { get; set; }
		
		[JsonIgnore]
		public EuroMoney EstimatedAmount
		{
			get
			{

				ThrowIfCannotBeEstimated();
				return _estimatedAmount;
			}
			set => _estimatedAmount = value;
		}

		private void ThrowIfCannotBeEstimated()
		{
			if (!CostCanBeEstimated) throw new InvalidOperationException("Cost cannot be estimated");
		}

		public DateTime NextBillDate { get; set; }
		public bool CostCanBeEstimated { get; set; }
	}
}