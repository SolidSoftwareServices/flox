using System.Threading.Tasks;
using EI.RP.DomainServices.Commands.Contracts.ChangeSmartPlanToStandard;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Commands.Contracts.ChangeSmartPlanToStandard
{
	[Explicit]
	class ChangeSmartPlanToStandardTests : DomainTests
	{
		[Test]
		public async Task CanExecute()
		{
			await LoginUser("smart_nonsmart@test.ie", "Test3333");
			await DomainCommandDispatcher.ExecuteAsync(
				new ChangeSmartPlanToStandardCommand("904754171"), true);
		}
	}
}
