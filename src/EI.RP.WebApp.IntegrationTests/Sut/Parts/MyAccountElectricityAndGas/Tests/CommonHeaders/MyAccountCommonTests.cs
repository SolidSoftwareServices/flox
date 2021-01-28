using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders
{
	abstract class MyAccountCommonTests<TPage>:WebAppPageTests<TPage> where TPage:MyAccountElectricityAndGasPage
	{
        protected AppUserConfigurator UserConfig { get; set; }
        protected static readonly IFixture Fixture = new Fixture().CustomizeDomainTypeBuilders();	
	}
}