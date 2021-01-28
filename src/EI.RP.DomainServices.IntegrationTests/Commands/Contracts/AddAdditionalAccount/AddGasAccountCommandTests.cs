using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts;
using EI.RP.DomainServices.Commands.Contracts.AddAdditionalAccount.Gas;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Commands.Contracts.AddAdditionalAccount
{

	[Explicit]
	class AddGasAccountCommandTests:DomainTests
	{
		[Test]
		public async Task ExecuteProductProposalFunction()
		{
			await LoginUser("DFNG1@test.ie", "Test3333");
			await DomainCommandDispatcher.ExecuteAsync(
				new AddGasAccountCommand("7538866", "904755886", 2233, PaymentSetUpType.SetUpNewDirectDebit, "IE29AIBK93115212345678", "John Doe"), true);
		}
	}
}
