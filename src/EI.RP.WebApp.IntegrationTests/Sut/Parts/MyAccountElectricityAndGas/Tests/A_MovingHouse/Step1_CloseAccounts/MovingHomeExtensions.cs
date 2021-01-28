using System.Linq;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainServices.Commands.Contracts.CloseAccounts.Model;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step1_CloseAccounts
{
	internal static class MovingHomeExtensions
	{
		public static ElectricityMeterReading ToElectricityMeterReading(this ElectricityAccountConfigurator configurator,
			StepCloseAccountPage page)
		{

			return new ElectricityMeterReading(configurator.Model.AccountNumber, ResolveValue(MeterType.Electricity24h), ResolveValue(MeterType.ElectricityDay), ResolveValue(MeterType.ElectricityNight));
			int? ResolveValue(MeterType meterType)
			{
				var infos = configurator.Premise.Devices.SelectMany(x=>x.Registers);
				var m1 = infos.SingleOrDefault(x => x.MeterType == meterType);
				int? resolveValue = null;
				if (m1 != null)
				{
					resolveValue = int.Parse(page.GetElectricityReadingInput(m1.MeterNumber).Value);
				}

				return resolveValue;
			}
		}
		public static GasMeterReading ToGasMeterReading(this GasAccountConfigurator configurator,
			StepCloseAccountPage page)
		{
			return new GasMeterReading(configurator.Model.AccountNumber,
				int.Parse(page
					.GetGasReadingInput(configurator.Premise.GasDevice().Registers.Single().MeterNumber).Value));

		}
	}
}