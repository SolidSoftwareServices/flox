using System.Linq;
using AutoFixture;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.SmartActivation.SmartActivationPlan;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Infrastructure
{
	class Step2SelectPlanDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;

		public Step2SelectPlanDataBuilder(AppUserConfigurator domainConfigurator)
		{
			_domainConfigurator = domainConfigurator;
		}
		public Step2SelectPlan.ScreenModel LastCreated { get; private set; }
		public SmartPlan SelectedPlan { get; private set; }
		public Step2SelectPlan.ScreenModel Create(string accountNumber)
		{
			var screenModel = _domainConfigurator.DomainFacade.ModelsBuilder.Build<Step2SelectPlan.ScreenModel>()
				.Create();
			SelectedPlan=_domainConfigurator.DomainFacade.ModelsBuilder.Build<SmartPlan>()
				.With(x=>x.PlanName,screenModel.SelectedPlanName)
				.Create();
			_domainConfigurator.DomainFacade.QueryResolver.ExpectQuery(new SmartActivationPlanQuery
				{
					AccountNumber = accountNumber,OnlyActive = true
				},SelectedPlan.ToOneItemArray().Union(_domainConfigurator.DomainFacade.ModelsBuilder.CreateMany<SmartPlan>(3))
			);
			return LastCreated = screenModel;
		}
	}
}