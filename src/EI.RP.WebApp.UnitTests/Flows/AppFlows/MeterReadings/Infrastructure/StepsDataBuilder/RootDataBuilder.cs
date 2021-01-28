using System;
using System.Linq;
using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MeterReadings.Infrastructure.StepsDataBuilder
{
	class RootDataBuilder
	{
		private readonly AppUserConfigurator _appUserConfigurator;

		public RootDataBuilder(AppUserConfigurator appUserConfigurator)
		{
			_appUserConfigurator = appUserConfigurator;
		}

		public MeterReadingFlowInitializer.RootScreenModel Create()
		{

			if (_appUserConfigurator.Accounts.Count() != 1)
			{
				throw new NotSupportedException();
			}

			var fixture = _appUserConfigurator.DomainFacade.ModelsBuilder;
			var accountConfigurator =
				(CommonElectricityAndGasAccountConfigurator) _appUserConfigurator.ElectricityAccount() ??
				_appUserConfigurator.GasAccount();

			_appUserConfigurator.DomainFacade.QueryResolver.ExpectQuery(new DevicesQuery
			{
				AccountNumber = accountConfigurator.Model.AccountNumber
			}, accountConfigurator.Premise.Devices);

			var result = fixture.Build<MeterReadingFlowInitializer.RootScreenModel>()
				.With(x => x.AccountNumber, accountConfigurator.Model.AccountNumber)
				.With(x => x.AccountType, accountConfigurator.AccountType)
				.With(x => x.Partner, accountConfigurator.Model.Partner)
				.With(x => x.StartType, MeterReadingFlowInitializer.StartType.ShowAndEditMeterReading)
				.Create();
			return result;
		}
	}
}