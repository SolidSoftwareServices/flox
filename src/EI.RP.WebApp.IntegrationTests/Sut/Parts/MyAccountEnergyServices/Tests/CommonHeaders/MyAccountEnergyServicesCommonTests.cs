using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Tests.CommonHeaders
{
	internal abstract class MyAccountEnergyServicesCommonTests<TPage> : WebAppPageTests<TPage>
		where TPage : MyAccountEnergyServicesPage
	{
		protected AppUserConfigurator UserConfig { get; set; }
	}
}