using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;
using System.Linq;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder
{
	class Step2InputPrnsDataBuilder
    {
		private readonly AppUserConfigurator _domainConfigurator;
		private readonly RootDataBuilder _rootDataBuilder;
		private readonly Step0OperationSelectionDataBuilder _step0Builder;
        private readonly Step1InputMoveOutPropertyDetailsDataBuilder _step1Builder;

        private readonly CommonElectricityAndGasAccountConfigurator _accountConfigurator;

        public Step2InputPrnsDataBuilder(AppUserConfigurator domainConfigurator,
			RootDataBuilder rootDataBuilder, Step0OperationSelectionDataBuilder step0Builder,
            Step1InputMoveOutPropertyDetailsDataBuilder step1Builder)
		{
			_domainConfigurator = domainConfigurator;

            _rootDataBuilder = rootDataBuilder;
			_step0Builder = step0Builder;
            _step1Builder = step1Builder;

            // TO-DO add support for muliple devices
            _accountConfigurator = _domainConfigurator.ElectricityAndGasAccountConfigurators.First();
        }

		public Step2InputPrns.ScreenModel LastCreated { get; private set; }

		public Step2InputPrns.ScreenModel Create()
		{
			return LastCreated = _domainConfigurator.DomainFacade.ModelsBuilder
				.Build<Step2InputPrns.ScreenModel>()
                .With(x => x.MovingHouseType, _rootDataBuilder.MovingType)
                .With(x => x.NewMPRN, (string) _accountConfigurator.NewPremise.ElectricityPrn)
                .With(x => x.NewGPRN, (string) _accountConfigurator.NewPremise.GasPrn)
                .With(x => x.PrnsAreFromSameAddress, true)
                .Create();
		}

        public Step2InputPrns.ScreenModel CleanNewPremisePRNs()
        {
            return LastCreated = _domainConfigurator.DomainFacade.ModelsBuilder
                .Build<Step2InputPrns.ScreenModel>()
                .With(x => x.MovingHouseType, _rootDataBuilder.MovingType)
                .Without(x => x.NewMPRN)
                .Without(x => x.NewGPRN)
                .Create();
        }
    }
}