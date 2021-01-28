using System;
using System.Linq;
using AutoFixture;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.Plan.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Plan.Infrastructure.Builders
{
	class RootDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;

		public RootDataBuilder(AppUserConfigurator domainConfigurator)
		{
			_domainConfigurator = domainConfigurator;
		}

		public PlanFlowInitializer.RootScreenModel LastCreated { get; private set; }

		public string AccountNumber => _domainConfigurator.ElectricityAccount().Model.AccountNumber;

		public PlanFlowInitializer.RootScreenModel Create()
		{
			var fixture = _domainConfigurator.DomainFacade.ModelsBuilder;

			return (LastCreated = fixture.Build<PlanFlowInitializer.RootScreenModel>()
				.With(x => x.AccountNumber, AccountNumber)
				.Create());
		}

	}
}