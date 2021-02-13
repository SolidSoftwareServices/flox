using NUnit.Framework;
using S3.UI.TestServices.Test;

namespace S3.App.AspNetCore3_1.IntegrationTests.Infrastructure
{
	[TestFixture]
	abstract class WebAppTests : WebAppTestsBase<PrototypeApp> 
	{
		protected WebAppTests() : base(PrototypeApp.StartNew)
		{
		}


	}
}