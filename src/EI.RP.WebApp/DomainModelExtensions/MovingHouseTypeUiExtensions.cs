using System.Linq;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.DomainModelExtensions
{
	public static class MovingHouseTypeUiExtensions
	{
		public static string ToPrnText(this MovingHouseType src)
		{
			string result = null;
			if (src.IsOneOf(
				MovingHouseType.MoveElectricityAndGas,
				MovingHouseType.MoveElectricityAndAddGas,
				MovingHouseType.MoveGasAndAddElectricity

			))
			{
				result = "MPRN/GPRN";
			}
			else if (src.IsOneOf(
				MovingHouseType.MoveElectricityAndCloseGas,
				MovingHouseType.MoveElectricity
			))
			{
				result = "MPRN";
			}
			else if (src== MovingHouseType.MoveGas)
			{

				result = "GPRN";
			}

			return result;
		}

		public static string ToDescriptionText(this MovingHouseType src)
		{
			if (src == MovingHouseType.MoveElectricityAndCloseGas)
				return "Moving Electricity & Closing Gas Account";
			if (src == MovingHouseType.CloseElectricityAndGas)
				return "Closing Electricity & Gas Accounts";
			if (src == MovingHouseType.MoveElectricityAndGas)
				return "Moving Electricity & Gas Accounts";
			if (src == MovingHouseType.MoveElectricity) return "Moving Electricity Account";
			if (src == MovingHouseType.CloseGas) return "Closing Gas Account";
			if (src == MovingHouseType.MoveGas) return "Moving Gas Account";
			if (src == MovingHouseType.CloseElectricity) return "Closing Electricity Account";
			if (src == MovingHouseType.MoveElectricityAndAddGas)
				return "Moving Electricity & Adding Gas Account";
			return "Moving Gas & Adding Electricity Account";
		}
	}
}