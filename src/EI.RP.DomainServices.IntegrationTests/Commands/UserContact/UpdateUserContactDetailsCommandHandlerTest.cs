using EI.RP.DomainServices.Commands.Users.UserContact;
using NUnit.Framework;
using System.Collections;
using System.Threading.Tasks;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;

namespace EI.RP.DomainServices.IntegrationTests.Commands.UserContact
{
    [Explicit]
    [TestFixture]
    public class UpdateUserContactDetailsCommandHandlerTest : DomainTests
    {

        public static IEnumerable CanExecuteTestCases()
        {
            UpdateUserContactDetailsCommand command = new UpdateUserContactDetailsCommand("902885821", "0899881297", "0899881288",
                "hop.mark2@test.com",  "hop.mark@test.com");

            yield return new TestCaseData(command);
        }

        [Test, TestCaseSource(nameof(CanExecuteTestCases))]
        public async Task CanExecute(UpdateUserContactDetailsCommand domainCommand)
        {
            await LoginUser("hop.mark@test.com", "Test1234");
            if (domainCommand != null)
            {
                await DomainCommandDispatcher.ExecuteAsync(domainCommand);
            }
        }
    }
}
