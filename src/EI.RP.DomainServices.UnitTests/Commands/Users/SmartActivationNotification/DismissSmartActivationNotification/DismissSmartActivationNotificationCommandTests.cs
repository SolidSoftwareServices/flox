using System;
using AutoFixture;
using EI.RP.DataModels.ResidentialPortal;
using EI.RP.DataServices;
using EI.RP.DomainServices.Commands.Users.SmartActivationNotification.DismissSmartActivationNotification;
using EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;

namespace EI.RP.DomainServices.UnitTests.Commands.Users.SmartActivationNotification.DismissSmartActivationNotification
{
	internal class DismissSmartActivationNotificationCommandTests : CommandHandlerTest<DismissSmartActivationNotificationCommandHandler,
	    DismissSmartActivationNotificationCommand>
    {
	    [Test]
	    public async Task DismissSmartActivationNotification()
	    {
		    var cmd = ArrangeAndGetCommand();

		    await Context.Sut.ExecuteAsync(cmd);

		    AssertCommandExecution();

		    void AssertCommandExecution()
		    {
			    var repo = Context.AutoMocker.GetMock<IResidentialPortalDataRepository>();
			    repo.Verify();
				repo.VerifyNoOtherCalls();
			}


			DismissSmartActivationNotificationCommand ArrangeAndGetCommand()
		    {
			    var accountNumber = Context.Fixture.Create<string>();
			    var userName = Context.Fixture.Create<string>();
			    Context.AutoMocker.GetMock<IUserSessionProvider>().SetupGet(x => x.UserName).Returns(userName);
			    var repoMock = Context.AutoMocker.GetMock<IResidentialPortalDataRepository>();
			    repoMock.Setup(x => x.Save(It.Is<SmartActivationNotificationDto>(model =>
				    model.AccountNumber == accountNumber &&
				    model.IsNotificationDismissed &&
				    model.UserName == userName))).Verifiable("Data repository was not called");
			    return new DismissSmartActivationNotificationCommand(accountNumber);
		    }
	    }
    }
}
