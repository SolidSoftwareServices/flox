using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder
{
	class AccountsForMovingTypeBuilder
	{
		public DomainFacade DomainFacade { get; }
		public MovingHouseType MovingType { get; }

		public AccountsForMovingTypeBuilder(MovingHouseType movingType)
		{
			DomainFacade = new DomainFacade();
			MovingType = movingType;
		}

		public AppUserConfigurator Create()
		{
			var userConfig = new AppUserConfigurator(DomainFacade);
			if (MovingType.IsMoveElectricity() || MovingType.IsCloseElectricity())
			{
				userConfig.AddElectricityAccount();
			}

			if (MovingType.IsMoveGas() || MovingType.IsCloseGas())
			{
				userConfig.AddGasAccount(duelFuelSisterAccount: MovingType.IsMoveElectricity()
					? userConfig.ElectricityAccount()
					: null);
			}
			return userConfig.Execute();
		}
	}
}