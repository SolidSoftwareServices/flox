using S3.UI.TestServices.Sut;
using S3.UI.TestServices.Test;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Infrastructure
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