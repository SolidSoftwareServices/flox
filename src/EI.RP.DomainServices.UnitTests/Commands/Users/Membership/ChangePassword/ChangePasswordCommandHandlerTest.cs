using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap;
using EI.RP.DataModels.Sap.CrmUmcUrm.Dtos;
using EI.RP.DataModels.Sap.UserManagement.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.Commands.Users.Membership.ChangePassword;
using EI.RP.DomainServices.Commands.Users.Session.CreateUserSession;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.TestServices;
using Moq;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Commands.Users.Membership.ChangePassword
{
	internal class ChangePasswordCommandHandlerTest : UnitTestFixture<ChangePasswordCommandHandlerTest.TestContext, ChangePasswordCommandHandler>
	{
		public class TestContext : UnitTestContext<ChangePasswordCommandHandler>
		{
			public TestContext() : base(new Fixture().CustomizeDomainTypeBuilders())
			{
			}
		}

		[Test]
		public async Task Execute_AdaptsPasswordToSapFormat()
		{
			var cmd = new ChangePasswordCommand(Context.Fixture.Create<EmailAddress>().ToString(), Context.Fixture.Create<string>(), "!a", Context.Fixture.Create<string>(), "00", Context.Fixture.Create<string>());
			string expectedPassword = cmd.NewPassword.AdaptToSapPasswordFormat();


			Context.AutoMocker.GetMock<ISapRepositoryOfCrmUmcUrm>().Setup(t =>
				t.Update(It.Is<UserRequestActivationRequestDto>(x => x.Password == null), true))
				.Returns(Task.CompletedTask);

			Context.AutoMocker.GetMock<ISapRepositoryOfUserManagement>().Setup(t =>
					t.Update(It.Is<CredentialDto>(x => x.Password == expectedPassword), true))
				.Returns(Task.CompletedTask);


			Context.AutoMocker.GetMock<ICommandHandler<CreateUserSessionCommand>>().Setup(t =>
					t.ExecuteAsync(It.Is<CreateUserSessionCommand>(x => x.Password == expectedPassword)))
				.Returns(Task.CompletedTask);

			await Context.Sut.ExecuteAsync(cmd);

			Context.AutoMocker.VerifyAll();
		}

		[Test]
		public async Task ItClearsSessionAfterMaxChangePasswordAttemptsHaveBeenReached()
		{
			const int maxChangePasswordAttemptsAllowed = 4;

			var userRepoMock = Context.AutoMocker.GetMock<ISapRepositoryOfUserManagement>();
			userRepoMock.Setup(x => 
					x.LoginUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
				.Throws(new DomainException(ResidentialDomainError.AuthenticationError));

			var counter = new ChangePasswordCommandHandler.ChangePasswordCounter();
			var userSessionMock = Context.AutoMocker.GetMock<IUserSessionProvider>();
			userSessionMock.Setup(x => 
				x.Get<ChangePasswordCommandHandler.ChangePasswordCounter>(GetUserSessionChangePasswordCounterKey())).Returns(counter);

			var cmd = new ChangePasswordCommand(
				Context.Fixture.Create<EmailAddress>().ToString(),
				"CurrentPasswordWrong",
				"newPassword", 
				Context.Fixture.Create<string>(),
				UserRequestStatusCode.Completed, 
				Context.Fixture.Create<string>());

			for (var i = 0; i <= maxChangePasswordAttemptsAllowed; i++)
			{
				Assert.ThrowsAsync<DomainException>(async() =>
					{
						await Context.Sut.ExecuteAsync(cmd);
					});
			}

			userRepoMock.Verify(x =>
				x.LoginUser(It.IsAny<string>(), It.IsAny<string>(), false),Times.Exactly(maxChangePasswordAttemptsAllowed));

			userRepoMock.Verify(x => 
				x.LoginUser(It.IsAny<string>(), It.IsAny<string>(), true), Times.Once());
		}

		[Test]
		public async Task ItResetsChangePasswordCounterAfterSuccessfulLogin()
		{
			var userRepoMock = Context.AutoMocker.GetMock<ISapRepositoryOfUserManagement>();
			userRepoMock.Setup(x => 
					x.LoginUser(It.IsAny<string>(), It.IsAny<string>(), false))
				.Returns(Task.FromResult(new SapSessionData()));

			var userSessionMock = Context.AutoMocker.GetMock<IUserSessionProvider>();
			userSessionMock.Setup(x => 
				x.Get<ChangePasswordCommandHandler.ChangePasswordCounter>(GetUserSessionChangePasswordCounterKey()))
				.Returns(new ChangePasswordCommandHandler.ChangePasswordCounter());

			var cmd = new ChangePasswordCommand(
				Context.Fixture.Create<EmailAddress>().ToString(),
				"CurrentPasswordWrong",
				"newPassword",
				Context.Fixture.Create<string>(),
				UserRequestStatusCode.Completed,
				Context.Fixture.Create<string>());

			await Context.Sut.ExecuteAsync(cmd);

			userSessionMock.Verify(x =>
				x.Remove(GetUserSessionChangePasswordCounterKey()), Times.Once);
		}

		private string GetUserSessionChangePasswordCounterKey()
		{
			return $"{nameof(ChangePasswordCommandHandler.ChangePasswordCounter)}";
		}
	}
}