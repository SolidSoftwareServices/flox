using System;
using System.Linq;
using AutoFixture;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder
{
	class RootDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;

		public RootDataBuilder(AppUserConfigurator domainConfigurator, MovingHouseType movingType)
		{
			_domainConfigurator = domainConfigurator;
			MovingType = movingType;
		}

		public MovingHouseType MovingType { get; }
		public FlowInitializer.RootScreenModel LastCreated { get; private set; }

		public FlowInitializer.RootScreenModel Create()
		{
			var fixture = _domainConfigurator.DomainFacade.ModelsBuilder;
			var inputFields = fixture.Build<EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions.InputFields>()
				.Without(x=>x.MeterReading24Hrs)
				.Without(x => x.MeterReadingDay)
				.Without(x => x.MeterReadingNight)
				.Without(x => x.MeterReadingNightStorageHeater)
				.Without(x => x.MeterReadingGas)
				.Create();
			return (LastCreated=fixture.Build<FlowInitializer.RootScreenModel>()
				.With(x=>x.UserMeterInputFields, inputFields)
				.With(x=>x.InitiatedFromAccountNumber,GetAccountInitiator(MovingType).AccountNumber)
				.With(x => x.ElectricityAccountNumber, _domainConfigurator.ElectricityAccount()?.Model.AccountNumber)
				.With(x => x.GasAccountNumber, _domainConfigurator.GasAccount()?.Model.AccountNumber)
				
					.Create());
		}

		private AccountInfo GetAccountInitiator(MovingHouseType movingType)
		{
			if (movingType.IsOneOf(MovingHouseType.MoveElectricity, MovingHouseType.MoveElectricityAndAddGas,
				MovingHouseType.MoveElectricityAndGas, MovingHouseType.MoveElectricityAndCloseGas, 
                MovingHouseType.CloseElectricity, MovingHouseType.CloseElectricityAndGas))
			{
				return _domainConfigurator.ElectricityAccount().Model;
			}

            if (movingType.IsOneOf(MovingHouseType.MoveGas, MovingHouseType.MoveGasAndAddElectricity,
				MovingHouseType.CloseGas))
				return _domainConfigurator.GasAccount().Model;

			throw new InvalidOperationException($"MovingHouseType: {movingType} not valid");
		}
	}
}