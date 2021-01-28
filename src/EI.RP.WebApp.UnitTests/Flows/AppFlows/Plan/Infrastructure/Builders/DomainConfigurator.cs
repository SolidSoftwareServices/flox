using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Plan.Infrastructure.Builders
{
	class DomainConfigurator
	{
		public DomainFacade DomainFacade { get; }

		public DomainConfigurator()
		{
			DomainFacade = new DomainFacade();
		}

		public AppUserConfigurator Create()
		{
			var userConfig = new AppUserConfigurator(DomainFacade);
			
			userConfig.AddElectricityAccount();
			
			return userConfig.Execute();
		}
	}
}