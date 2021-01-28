using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.Commands.Users.Session.CreateUserSession;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.TestServices;
using Moq;
using NUnit.Framework;
using SapSessionData = EI.RP.DataModels.Sap.SapSessionData;

namespace EI.RP.DomainServices.UnitTests.Commands.Users.Session.CreateUserSession
{
	internal class CreateUserSessionCommandHandlerTest : UnitTestFixture<CreateUserSessionCommandHandlerTest.TestContext,CreateUserSessionCommandHandler>
	{
		public class TestContext : UnitTestContext<CreateUserSessionCommandHandler>
		{
			public TestContext() : base(new Fixture().CustomizeDomainTypeBuilders())
			{
			}
		}

		[Test, Theory]
		public async Task Execute_AdaptsPasswordToSapFormat(bool adapt)
		{
			var cmd = new CreateUserSessionCommand(Context.Fixture.Create<EmailAddress>().ToString(), "!a",adapt);
			var expectedPassword = adapt ? cmd.Password.AdaptToSapPasswordFormat() : cmd.Password;

			Context.AutoMocker.GetMock<ISapRepositoryOfUserManagement>().Setup(t =>
				t.LoginUser(It.Is<string>(x => x == cmd.UserEmail.ToString().AdaptToSapUserNameFormat()),
					It.Is<string>(x => x == expectedPassword),true)).ReturnsAsync(Context.Fixture.Build<SapSessionData>()
				.With(x => x.SapUserRole, ResidentialPortalUserRole.OnlineUser).Create());

			await Context.Sut.ExecuteAsync(cmd);

			Context.AutoMocker.VerifyAll();
		}
	}
}