using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;
using System.Linq;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder
{
	class Step2ConfirmAddressDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;
		private readonly RootDataBuilder _rootDataBuilder;
		private readonly CommonElectricityAndGasAccountConfigurator _accountConfigurator;

        public Step2ConfirmAddressDataBuilder(
	        AppUserConfigurator domainConfigurator,
			RootDataBuilder rootDataBuilder)
		{
			_domainConfigurator = domainConfigurator;
			_rootDataBuilder = rootDataBuilder;


            // TO-DO add support for muliple devices
            _accountConfigurator = _domainConfigurator.ElectricityAndGasAccountConfigurators.First();
        }

		public Step2ConfirmAddress.ScreenModel LastCreated { get; private set; }

		public Step2ConfirmAddress.ScreenModel Create()
		{
			return LastCreated = _domainConfigurator.DomainFacade.ModelsBuilder
				.Build<Step2ConfirmAddress.ScreenModel>()
                .With(x => x.MovingHouseType, _rootDataBuilder.MovingType)
                .With(x => x.NewMPRN, (string) _accountConfigurator.NewPremise.ElectricityPrn)
                .With(x => x.NewGPRN, (string) _accountConfigurator.NewPremise.GasPrn)
				.Create();
		}
	}
}