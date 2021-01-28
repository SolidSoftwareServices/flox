using System.Collections;
using System.Threading.Tasks;
using Autofac;
using EI.RP.CoreServices.Http.Server;
using EI.RP.CoreServices.Platform;
using EI.RP.DomainServices.Commands.BusinessPartner.Deregister;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Commands.BusinessPartner
{
	[Ignore("TODO")]
	[TestFixture]
	public class DeregisterBusinessPartnerCommandTests : DomainTests
	{
		private bool _wasInternal;

		[OneTimeSetUp]
		public void OnOneTimeSetUp()
		{
			var preProdSettings = (TestSettings) AssemblySetUp.Container.Value.Resolve<IPlatformSettings>();
			_wasInternal = preProdSettings.IsInternalDeployment;
			preProdSettings.IsInternalDeployment = true;
		}

		[OneTimeTearDown]
		public void OnOneTimeTearDown()
		{
			((TestSettings) AssemblySetUp.Container.Value.Resolve<IPlatformSettings>()).IsInternalDeployment =
				_wasInternal;
		}

		public static IEnumerable CommandCases()
		{
			yield return new TestCaseData("furlong_d", "Init1234", "1001870277", "duelfuel@esb.ie");
		}

		[Test]
		[TestCaseSource(nameof(CommandCases))]
		public async Task QueryReturnsCorrectNumberOfAccounts(string userName, string password, string partnerNum,
			string bpUserName)
		{
			await LoginUser(userName, password);

			var command = new DeRegisterBusinessPartnerCommand(partnerNum, false);
			await DomainCommandDispatcher.ExecuteAsync(command);
		}
	}
}