using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EI.RP.DomainServices.Commands.Users.MarketingPreferences;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Commands.MarketingPreferences
{
	[Explicit]
	[TestFixture]
	public class UpdateMarketingPreferencesCommandHandlerTest : DomainTests
	{

		public static IEnumerable CanExecuteTestCases()
		{
			UpdateMarketingPreferencesCommand command = new UpdateMarketingPreferencesCommand("902885821", true, false, true, false, true, false);

			yield return new TestCaseData(command);
		}

		[Test, TestCaseSource(nameof(CanExecuteTestCases))]
		public async Task CanExecute(UpdateMarketingPreferencesCommand domainCommand)
		{
			await LoginUser("hop.mark@test.com", "Test1234");
			if (domainCommand != null)
			{
				await DomainCommandDispatcher.ExecuteAsync(domainCommand);
			}
		}
	}
}
