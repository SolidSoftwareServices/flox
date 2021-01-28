using System;
using System.Threading.Tasks;
using EI.RP.UI.TestServices.Sut;
using EI.RP.UI.TestServices.Test;
using EI.RP.WebApp.IntegrationTests.Sut;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Infrastructure
{
	

	[TestFixture]
	abstract class WebAppPageTests<TPage>: WebAppPageTestsBase<ResidentialPortalApp, TPage> where TPage : ISutPage
	{
		protected WebAppPageTests(ResidentialPortalDeploymentType runAsDeploymentType=ResidentialPortalDeploymentType.Public) : base(()=>ResidentialPortalApp.StartNew(runAsDeploymentType))
		{
		}
    }
}