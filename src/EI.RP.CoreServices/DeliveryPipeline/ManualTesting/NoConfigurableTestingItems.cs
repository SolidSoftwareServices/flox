using System;
using EI.RP.CoreServices.System;

namespace EI.RP.CoreServices.DeliveryPipeline.ManualTesting
{
	/// <summary>
	/// this is the implementation injected in non-development environments: pre-prod and prod...
	/// </summary>
	internal class NoConfigurableTestingItems : IConfigurableTestingItems
	{
		private static readonly string BadUsageMessage = $"The use of the interface {nameof(IConfigurableTestingItems)} is illegal in environments following the tests environment in the delivery pipeline";

		public int SimulateAppWorkloadDelaySeconds
		{
			get => 0;
			set => throw new InvalidOperationException(BadUsageMessage);
		}

		public bool SimulateConsumptionDataFailure
		{
			get => false;
			set =>  throw new InvalidOperationException(BadUsageMessage);
		}


		public bool CanShowCostToDate
		{
			get => false;
			set => throw new InvalidOperationException(BadUsageMessage);
		}

		public EuroMoney CostToDateAmount
		{
			get => EuroMoney.Zero;
			set => throw new InvalidOperationException(BadUsageMessage);
		}

		public DateTime CostToDateSince
		{
			get => DateTime.MinValue;
			set => throw new InvalidOperationException(BadUsageMessage);
		}
	}
}