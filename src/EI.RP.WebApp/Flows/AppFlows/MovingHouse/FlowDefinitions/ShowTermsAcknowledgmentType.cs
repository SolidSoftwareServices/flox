using System;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions
{
	public enum ShowTermsAcknowledgmentType
	{
		None,
		ShowElectricity,
		ShowGas,
		ShowBoth
	}

	internal static class MovingHouseTypeExtensions
	{
		public static ShowTermsAcknowledgmentType ToShowTermsAcknowledgementType(this MovingHouseType src)
		{
			if (src.IsOneOf(MovingHouseType.MoveElectricity ,MovingHouseType.MoveElectricityAndCloseGas ,MovingHouseType.CloseElectricity))
			{
				return ShowTermsAcknowledgmentType.ShowElectricity;
			}

			if (src.IsOneOf( MovingHouseType.MoveGas ,MovingHouseType.CloseGas))
			{
				return ShowTermsAcknowledgmentType.ShowGas;
			}

			if (src.IsOneOf( MovingHouseType.MoveElectricityAndAddGas ,
			    MovingHouseType.MoveElectricityAndGas ,
			    MovingHouseType.MoveGasAndAddElectricity ,
			    MovingHouseType.CloseElectricityAndGas))
			{
				return ShowTermsAcknowledgmentType.ShowBoth;
			}

			throw new ArgumentOutOfRangeException(src);
		}
	}
}