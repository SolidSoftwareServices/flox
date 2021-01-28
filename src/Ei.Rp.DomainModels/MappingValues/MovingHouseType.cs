using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class MovingHouseType : TypedStringValue<MovingHouseType>
	{


		[JsonConstructor]
		private MovingHouseType()
		{
		}

		private MovingHouseType(string value) : base(value)
		{
		}

		public static readonly MovingHouseType MoveElectricity = new MovingHouseType(nameof(MoveElectricity));
		public static readonly MovingHouseType MoveElectricityAndCloseGas = new MovingHouseType(nameof(MoveElectricityAndCloseGas));
		public static readonly MovingHouseType MoveElectricityAndGas = new MovingHouseType(nameof(MoveElectricityAndGas));

		public static readonly MovingHouseType MoveGas = new MovingHouseType(nameof(MoveGas));


		public static readonly MovingHouseType MoveGasAndAddElectricity = new MovingHouseType(nameof(MoveGasAndAddElectricity));
		public static readonly MovingHouseType MoveElectricityAndAddGas = new MovingHouseType(nameof(MoveElectricityAndAddGas));


		public static readonly MovingHouseType CloseGas = new MovingHouseType(nameof(CloseGas));
		public static readonly MovingHouseType CloseElectricity = new MovingHouseType(nameof(CloseElectricity));
		public static readonly MovingHouseType CloseElectricityAndGas = new MovingHouseType(nameof(CloseElectricityAndGas));

		public bool IsMoveElectricity()
		{
			return this.IsOneOf(MoveElectricity, MoveElectricityAndAddGas, MoveElectricityAndCloseGas,
				MoveElectricityAndGas);
		}

		public bool IsAddElectricity()
		{
			return this.IsOneOf(MoveGasAndAddElectricity);
		}

		public bool IsMoveGas()
		{
			return this.IsOneOf(MoveGasAndAddElectricity, MoveGas, MoveElectricityAndGas);
		}

		public bool IsAddGas()
		{
			return this.IsOneOf(MoveElectricityAndAddGas);
		}

		public bool IsCloseElectricity()
		{
			return this.IsOneOf(CloseElectricity,CloseElectricityAndGas);
		}

		public bool IsCloseGas()
		{
			return this.IsOneOf(CloseGas, CloseElectricityAndGas, MoveElectricityAndCloseGas);
		}

		public bool IsCloseOnly()
		{
			return IsOneOf(CloseGas, CloseElectricityAndGas, CloseElectricity, CloseElectricityAndGas);
		}
	}

}