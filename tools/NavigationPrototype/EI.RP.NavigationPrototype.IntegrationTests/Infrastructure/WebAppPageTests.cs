using EI.RP.UI.TestServices.Sut;
using EI.RP.UI.TestServices.Test;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Infrastructure
{
	

	[TestFixture]
	abstract class WebAppPageTests<TPage>: WebAppPageTestsBase<PrototypeApp,TPage> where TPage : ISutPage
	{
		protected WebAppPageTests() : base(PrototypeApp.StartNew)
		{
		}


	}

	[TestFixture]
	abstract class WebAppTests : WebAppTestsBase<PrototypeApp> 
	{
		protected WebAppTests() : base(PrototypeApp.StartNew)
		{
		}


	}
}